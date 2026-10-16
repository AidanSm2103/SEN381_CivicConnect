namespace CivicConnect.Tests
{
    /// <summary>OWNER: Enrique de Sousa. Assignment and workflow rules: guarded transitions, atomic audit + feedback (D-004, D-005, NFR-010, FR-005, FR-009..FR-012).</summary>
    public class RequestWorkflowServiceTests
    {
        private static readonly byte[] Version = { 1 };

        private static AssignRequestCommand Assign(ServiceRequest r, int staffId) =>
            new() { RequestId = r.Id, AssigneeStaffId = staffId, RowVersion = Version };

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "NFR-010"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public async Task Assign_WritesHistoryAndFeedbackTogether()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsStaff(2));
            var request = h.AddRequest(RequestStatus.Submitted, categoryId: 1);

            var result = await h.Workflow.AssignAsync(Assign(request, 2));

            Assert.True(result.Succeeded);
            Assert.Equal(RequestStatus.Assigned, request.Status);
            Assert.Equal(2, request.AssignedStaffId);
            var history = Assert.Single(h.AuditLog.Histories);
            Assert.Equal(RequestStatus.Submitted, history.FromStatus);
            Assert.Equal(RequestStatus.Assigned, history.ToStatus);
            Assert.Equal(2, history.ChangedByUserId);
            var feedback = Assert.Single(h.AuditLog.Comments);
            Assert.Equal(CommentKind.Feedback, feedback.Kind);
            Assert.True(feedback.VisibleToRequester);
            Assert.Equal(1, h.UnitOfWork.SaveCount);                     // everything committed in ONE save
            Assert.Equal(Version, Assert.Single(h.Requests.ExpectedRowVersions).RowVersion);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-009"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task Assign_WhenConcurrencyConflict_ReturnsConflictAndWritesNothing()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsStaff(2));
            var request = h.AddRequest(RequestStatus.Submitted, categoryId: 1);
            h.UnitOfWork.ThrowConcurrencyConflict = true;

            var result = await h.Workflow.AssignAsync(Assign(request, 2));

            Assert.True(result.Failed);
            Assert.Equal(ErrorCodes.ConcurrencyConflict, result.ErrorCode);
            Assert.Equal(0, h.UnitOfWork.SaveCount);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-009"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task Accept_WhenSomeoneAlreadyHoldsTheRequest_ReportsAlreadyAssigned()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsStaff(2));
            var request = h.AddRequest(RequestStatus.Assigned, categoryId: 1, assignedTo: 4);

            var result = await h.Workflow.AcceptAsync(new AcceptRequestCommand { RequestId = request.Id, RowVersion = Version });

            Assert.Equal(ErrorCodes.AlreadyAssigned, result.ErrorCode);
            Assert.Equal(4, request.AssignedStaffId);
            Assert.Empty(h.AuditLog.Histories);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-009"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public async Task Accept_AssignsTheCurrentStaffMember()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsStaff(2));
            var request = h.AddRequest(RequestStatus.Submitted, categoryId: 1);

            var result = await h.Workflow.AcceptAsync(new AcceptRequestCommand { RequestId = request.Id, RowVersion = Version });

            Assert.True(result.Succeeded);
            Assert.Equal(2, request.AssignedStaffId);
            Assert.Equal(RequestStatus.Assigned, request.Status);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-009"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task Assign_ToStaffWhoDoNotHandleTheCategory_IsRejected()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsStaff(2));
            var request = h.AddRequest(RequestStatus.Submitted, categoryId: 1);

            var result = await h.Workflow.AssignAsync(Assign(request, 5));       // staff 5 only handles category 2

            Assert.Equal(ErrorCodes.ValidationFailed, result.ErrorCode);
            Assert.Equal(RequestStatus.Submitted, request.Status);
            Assert.Null(request.AssignedStaffId);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-009"), Trait("Priority", "Medium"), Trait("Type", "Negative")]
        public async Task Assign_ToNonStaffOrUnknownUser_IsRejected()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsStaff(2));
            var request = h.AddRequest(RequestStatus.Submitted, categoryId: 1);

            Assert.Equal(ErrorCodes.ValidationFailed, (await h.Workflow.AssignAsync(Assign(request, 1))).ErrorCode);    // a requester
            Assert.Equal(ErrorCodes.ValidationFailed, (await h.Workflow.AssignAsync(Assign(request, 999))).ErrorCode);  // nobody
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "NFR-003"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task Management_CannotAssign_AndNothingIsWritten()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsManager());
            var request = h.AddRequest(RequestStatus.Submitted, categoryId: 1);

            var result = await h.Workflow.AssignAsync(Assign(request, 2));

            Assert.Equal(ErrorCodes.Forbidden, result.ErrorCode);
            Assert.Empty(h.AuditLog.Histories);
            Assert.Empty(h.AuditLog.Comments);
            Assert.Equal(0, h.UnitOfWork.SaveCount);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-010"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task UnknownRequest_IsNotFound()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsStaff(2));

            Assert.Equal(ErrorCodes.NotFound, (await h.Workflow.AssignAsync(new AssignRequestCommand { RequestId = 404, AssigneeStaffId = 2, RowVersion = Version })).ErrorCode);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-010"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public async Task FullLifecycle_Submitted_To_Closed_WritesOneHistoryRowAndOneFeedbackPerStep()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsStaff(2));
            var request = h.AddRequest(RequestStatus.Submitted, categoryId: 1);

            Assert.True((await h.Workflow.AcceptAsync(new AcceptRequestCommand { RequestId = request.Id, RowVersion = Version })).Succeeded);
            Assert.True((await h.Workflow.ChangeStatusAsync(new ChangeStatusCommand { RequestId = request.Id, TargetStatus = RequestStatus.InProgress, RowVersion = Version })).Succeeded);
            Assert.True((await h.Workflow.ResolveAsync(new ResolveRequestCommand { RequestId = request.Id, ResolutionNote = "Replaced the bulb", RowVersion = Version })).Succeeded);
            Assert.True((await h.Workflow.CloseAsync(new CloseRequestCommand { RequestId = request.Id, RowVersion = Version })).Succeeded);

            Assert.Equal(RequestStatus.Closed, request.Status);
            Assert.NotNull(request.AssignedAt);
            Assert.NotNull(request.ResolvedAt);
            Assert.NotNull(request.ClosedAt);
            Assert.Equal(4, h.AuditLog.Histories.Count);
            Assert.Equal(4, h.AuditLog.Comments.Count(c => c.Kind == CommentKind.Feedback));
            Assert.Equal(4, h.UnitOfWork.SaveCount);                     // one transaction per step
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-011"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task Resolve_WithoutANote_IsRejected_AndStatusIsUnchanged()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsStaff(2));
            var request = h.AddRequest(RequestStatus.InProgress, categoryId: 1, assignedTo: 2);

            var result = await h.Workflow.ResolveAsync(new ResolveRequestCommand { RequestId = request.Id, ResolutionNote = "   ", RowVersion = Version });

            Assert.Equal(ErrorCodes.ValidationFailed, result.ErrorCode);
            Assert.Equal(RequestStatus.InProgress, request.Status);
            Assert.Equal(0, h.UnitOfWork.SaveCount);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-011"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public async Task Resolve_StoresTheNoteAsStaffOnly_AndKeepsItOutOfRequesterFeedback()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsStaff(2));
            var request = h.AddRequest(RequestStatus.InProgress, categoryId: 1, assignedTo: 2);

            await h.Workflow.ResolveAsync(new ResolveRequestCommand { RequestId = request.Id, ResolutionNote = "Replaced the fuse", RowVersion = Version });

            var note = Assert.Single(h.AuditLog.Comments, c => c.Kind == CommentKind.ResolutionNote);
            Assert.False(note.VisibleToRequester);
            Assert.Equal("Replaced the fuse", note.Body);
            var feedback = Assert.Single(h.AuditLog.Comments, c => c.Kind == CommentKind.Feedback);
            Assert.DoesNotContain("fuse", feedback.Body);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-010"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task ChangeStatus_OtherThanStart_IsRejected()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsStaff(2));
            var request = h.AddRequest(RequestStatus.Assigned, categoryId: 1, assignedTo: 2);

            var result = await h.Workflow.ChangeStatusAsync(new ChangeStatusCommand { RequestId = request.Id, TargetStatus = RequestStatus.Closed, RowVersion = Version });

            Assert.Equal(ErrorCodes.InvalidTransition, result.ErrorCode);
            Assert.Equal(RequestStatus.Assigned, request.Status);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-011"), Trait("Priority", "Medium"), Trait("Type", "Positive")]
        public async Task AddNote_CreatesStaffOnlyComment_WithoutHistoryOrFeedback()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsStaff(2));
            var request = h.AddRequest(RequestStatus.InProgress, categoryId: 1, assignedTo: 2);

            var result = await h.Workflow.AddNoteAsync(new AddNoteCommand { RequestId = request.Id, Body = "Waiting for parts" });

            Assert.True(result.Succeeded);
            var note = Assert.Single(h.AuditLog.Comments);
            Assert.False(note.VisibleToRequester);
            Assert.Empty(h.AuditLog.Histories);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-012"), Trait("Priority", "Medium"), Trait("Type", "Negative")]
        public async Task AddNote_OnClosedRequest_OrWithBlankBody_IsRejected()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsStaff(2));
            var closed = h.AddRequest(RequestStatus.Closed, categoryId: 1, assignedTo: 2);
            var open = h.AddRequest(RequestStatus.InProgress, categoryId: 1, assignedTo: 2);

            Assert.Equal(ErrorCodes.InvalidTransition, (await h.Workflow.AddNoteAsync(new AddNoteCommand { RequestId = closed.Id, Body = "late" })).ErrorCode);
            Assert.Equal(ErrorCodes.ValidationFailed, (await h.Workflow.AddNoteAsync(new AddNoteCommand { RequestId = open.Id, Body = " " })).ErrorCode);
            Assert.Empty(h.AuditLog.Comments);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "NFR-003"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public async Task AvailableActions_FollowRoleScopeAndStatus()
        {
            var staff = new ServiceHarness(FakeCurrentUser.AsStaff(2));
            var submitted = staff.AddRequest(RequestStatus.Submitted, categoryId: 1);
            var actions = await staff.Workflow.GetAvailableActionsAsync(submitted);
            Assert.True(actions.CanAccept && actions.CanAssign && actions.CanAddNote);
            Assert.False(actions.CanStart || actions.CanResolve || actions.CanClose);

            var inProgress = staff.AddRequest(RequestStatus.InProgress, categoryId: 1, assignedTo: 2);
            actions = await staff.Workflow.GetAvailableActionsAsync(inProgress);
            Assert.True(actions.CanResolve);
            Assert.False(actions.CanAccept || actions.CanAssign || actions.CanStart || actions.CanClose);

            var outOfScope = staff.AddRequest(RequestStatus.Submitted, categoryId: 2);
            actions = await staff.Workflow.GetAvailableActionsAsync(outOfScope);
            Assert.False(actions.CanAccept || actions.CanAssign || actions.CanAddNote);

            var closed = staff.AddRequest(RequestStatus.Closed, categoryId: 1, assignedTo: 2);
            Assert.False((await staff.Workflow.GetAvailableActionsAsync(closed)).CanAddNote);

            var manager = new ServiceHarness(FakeCurrentUser.AsManager());
            var any = manager.AddRequest(RequestStatus.Submitted, categoryId: 1);
            actions = await manager.Workflow.GetAvailableActionsAsync(any);
            Assert.False(actions.CanAccept || actions.CanAssign || actions.CanStart || actions.CanResolve || actions.CanClose || actions.CanAddNote);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-009"), Trait("Priority", "Medium"), Trait("Type", "Positive")]
        public async Task ListAssignableStaff_IsForStaffAndManagementOnly()
        {
            Assert.Equal(3, (await new ServiceHarness(FakeCurrentUser.AsStaff(2)).Workflow.ListAssignableStaffAsync()).Count);
            Assert.Equal(3, (await new ServiceHarness(FakeCurrentUser.AsManager()).Workflow.ListAssignableStaffAsync()).Count);
            Assert.Empty(await new ServiceHarness(FakeCurrentUser.AsRequester(1)).Workflow.ListAssignableStaffAsync());
        }
    }
}
