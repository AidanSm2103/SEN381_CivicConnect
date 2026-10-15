namespace CivicConnect.Tests
{
    /// <summary>OWNER: Enrique de Sousa. Submit / view / list / search rules (FR-001..FR-004, FR-006..FR-008, FR-011) on in-memory fakes.</summary>
    public class RequestServiceTests
    {
        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-001"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public async Task Submit_Valid_CreatesRequestAndFirstHistoryRowInOneTransaction()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsRequester(1));

            var result = await h.RequestService.SubmitAsync(new SubmitRequestCommand { CategoryId = 1, Description = "  Broken light  " });

            Assert.True(result.Succeeded);
            var request = Assert.Single(h.Requests.Items);
            Assert.Equal("CC-2026-000001", request.ReferenceNo);
            Assert.Equal("Broken light", request.Description);          // trimmed
            Assert.Equal(RequestStatus.Submitted, request.Status);
            Assert.Equal(1, request.RequesterId);
            var history = Assert.Single(h.AuditLog.Histories);
            Assert.Null(history.FromStatus);
            Assert.Equal(RequestStatus.Submitted, history.ToStatus);
            Assert.Equal(1, h.UnitOfWork.SaveCount);                    // request + audit row saved together, once
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-002"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task Submit_InactiveCategory_IsRejectedAndNothingSaved()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsRequester(1));

            var result = await h.RequestService.SubmitAsync(new SubmitRequestCommand { CategoryId = 99, Description = "Anything" });

            Assert.True(result.Failed);
            Assert.Equal(ErrorCodes.ValidationFailed, result.ErrorCode);
            Assert.Empty(h.Requests.Items);
            Assert.Empty(h.AuditLog.Histories);
            Assert.Equal(0, h.UnitOfWork.SaveCount);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "NFR-003"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task Submit_AsStaff_IsForbidden()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsStaff(2));

            var result = await h.RequestService.SubmitAsync(new SubmitRequestCommand { CategoryId = 1, Description = "x" });

            Assert.Equal(ErrorCodes.Forbidden, result.ErrorCode);
            Assert.Empty(h.Requests.Items);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "NFR-002"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task Submit_Anonymous_IsUnauthenticated()
        {
            var h = new ServiceHarness(FakeCurrentUser.Anonymous());

            var result = await h.RequestService.SubmitAsync(new SubmitRequestCommand { CategoryId = 1, Description = "x" });

            Assert.Equal(ErrorCodes.Unauthenticated, result.ErrorCode);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-001"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public async Task Submit_TwoRequests_GetDifferentReferenceNumbers()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsRequester(1));

            await h.RequestService.SubmitAsync(new SubmitRequestCommand { CategoryId = 1, Description = "one" });
            await h.RequestService.SubmitAsync(new SubmitRequestCommand { CategoryId = 2, Description = "two" });

            Assert.Equal(2, h.Requests.Items.Select(r => r.ReferenceNo).Distinct().Count());
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-008"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task GetDetails_AnotherRequestersRequest_IsForbidden_AndUnknownIdIsNotFound()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsRequester(7));
            var someoneElses = h.AddRequest(requesterId: 1);

            Assert.Equal(ErrorCodes.Forbidden, (await h.RequestService.GetDetailsAsync(someoneElses.Id)).ErrorCode);
            Assert.Equal(ErrorCodes.NotFound, (await h.RequestService.GetDetailsAsync(12345)).ErrorCode);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-011"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task GetDetails_ForRequester_HidesStaffOnlyNotes_ButStaffSeeThem()
        {
            var asRequester = new ServiceHarness(FakeCurrentUser.AsRequester(1));
            var request = asRequester.AddRequest(requesterId: 1);
            request.Comments.Add(new RequestComment { Kind = CommentKind.Feedback, Body = "visible", VisibleToRequester = true });
            request.Comments.Add(new RequestComment { Kind = CommentKind.ResolutionNote, Body = "internal", VisibleToRequester = false });

            var seenByRequester = (await asRequester.RequestService.GetDetailsAsync(request.Id)).Value!;
            Assert.Equal("visible", Assert.Single(seenByRequester.Comments).Body);

            var asStaff = new ServiceHarness(FakeCurrentUser.AsStaff(2));
            var request2 = asStaff.AddRequest(requesterId: 1);
            request2.Comments.Add(new RequestComment { Body = "visible", VisibleToRequester = true });
            request2.Comments.Add(new RequestComment { Body = "internal", VisibleToRequester = false });

            Assert.Equal(2, (await asStaff.RequestService.GetDetailsAsync(request2.Id)).Value!.Comments.Count);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-003"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public async Task ListMine_ReturnsOnlyTheCurrentRequestersRequests()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsRequester(1));
            h.AddRequest(requesterId: 1);
            h.AddRequest(requesterId: 1);
            h.AddRequest(requesterId: 8);

            var mine = await h.RequestService.ListMineAsync();

            Assert.Equal(2, mine.Count);
            Assert.All(mine, r => Assert.Equal(1, r.RequesterId));
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-006"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task Search_Staff_OnlySeesTheirCategories_ManagementSeesAll_RequesterSeesNothing()
        {
            var staff = new ServiceHarness(FakeCurrentUser.AsStaff(2));      // category 1 only
            staff.AddRequest(categoryId: 1);
            staff.AddRequest(categoryId: 2);
            Assert.Equal(1, (await staff.RequestService.SearchAsync(new RequestSearchCriteria())).TotalCount);

            // Filtering on a category outside the scope is simply empty.
            Assert.Equal(0, (await staff.RequestService.SearchAsync(new RequestSearchCriteria { CategoryId = 2 })).TotalCount);

            var manager = new ServiceHarness(FakeCurrentUser.AsManager());
            manager.AddRequest(categoryId: 1);
            manager.AddRequest(categoryId: 2);
            Assert.Equal(2, (await manager.RequestService.SearchAsync(new RequestSearchCriteria())).TotalCount);

            var requester = new ServiceHarness(FakeCurrentUser.AsRequester(1));
            requester.AddRequest();
            Assert.Equal(0, (await requester.RequestService.SearchAsync(new RequestSearchCriteria())).TotalCount);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-012"), Trait("Priority", "Medium"), Trait("Type", "Positive")]
        public async Task Search_ClosedRequests_AreHiddenUnlessRequested()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsManager());
            h.AddRequest(RequestStatus.Submitted);
            h.AddRequest(RequestStatus.Closed);

            Assert.Equal(1, (await h.RequestService.SearchAsync(new RequestSearchCriteria())).TotalCount);
            Assert.Equal(2, (await h.RequestService.SearchAsync(new RequestSearchCriteria { IncludeClosed = true })).TotalCount);
        }
    }
}
