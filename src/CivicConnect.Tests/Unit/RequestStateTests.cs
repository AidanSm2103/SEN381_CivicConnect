namespace CivicConnect.Tests
{
    /// <summary>OWNER: Enrique de Sousa. State pattern (D-005): which moves each status allows and what each move changes.</summary>
    public class RequestStateTests
    {
        private static readonly DateTime Now = new(2026, 10, 19, 9, 0, 0, DateTimeKind.Utc);

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-010"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public void Submitted_Assign_SetsAssigneeAndMovesToAssigned()
        {
            var request = TestData.Request(RequestStatus.Submitted);

            var result = new SubmittedState().Assign(request, 2, Now);

            Assert.True(result.Succeeded);
            Assert.Equal(RequestStatus.Assigned, request.Status);
            Assert.Equal(2, request.AssignedStaffId);
            Assert.Equal(Now, request.AssignedAt);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-010"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public void Closed_AnyAction_FailsWithInvalidTransition()
        {
            var request = TestData.Request(RequestStatus.Closed);
            var state = new ClosedState();

            var results = new[]
            {
                state.Assign(request, 2, Now),
                state.Start(request),
                state.Resolve(request, Now),
                state.Close(request, Now)
            };

            Assert.All(results, r =>
            {
                Assert.True(r.Failed);
                Assert.Equal(ErrorCodes.InvalidTransition, r.ErrorCode);
            });
            Assert.Equal(RequestStatus.Closed, request.Status);
            Assert.Null(request.AssignedStaffId);
            Assert.Null(request.ResolvedAt);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-010"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public void Submitted_CannotSkipToResolved()
        {
            var request = TestData.Request(RequestStatus.Submitted);
            var state = new SubmittedState();

            Assert.False(state.CanMoveTo(RequestStatus.Resolved));
            var result = state.Resolve(request, Now);

            Assert.True(result.Failed);
            Assert.Equal(ErrorCodes.InvalidTransition, result.ErrorCode);
            Assert.Equal(RequestStatus.Submitted, request.Status);
            Assert.Null(request.ResolvedAt);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-009"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public void Submitted_AlreadyHoldingAnAssignee_ReportsAlreadyAssigned()
        {
            var request = TestData.Request(RequestStatus.Submitted);
            request.AssignedStaffId = 4;

            var result = new SubmittedState().Assign(request, 2, Now);

            Assert.True(result.Failed);
            Assert.Equal(ErrorCodes.AlreadyAssigned, result.ErrorCode);
            Assert.Equal(4, request.AssignedStaffId);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-010"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public void ForwardMoves_SetStatusAndTimestamps()
        {
            var request = TestData.Request(RequestStatus.Assigned);

            Assert.True(new AssignedState().Start(request).Succeeded);
            Assert.Equal(RequestStatus.InProgress, request.Status);

            Assert.True(new InProgressState().Resolve(request, Now).Succeeded);
            Assert.Equal(RequestStatus.Resolved, request.Status);
            Assert.Equal(Now, request.ResolvedAt);

            Assert.True(new ResolvedState().Close(request, Now.AddHours(1)).Succeeded);
            Assert.Equal(RequestStatus.Closed, request.Status);
            Assert.Equal(Now.AddHours(1), request.ClosedAt);
        }

        [Theory]
        [InlineData(RequestStatus.Submitted)]
        [InlineData(RequestStatus.Assigned)]
        [InlineData(RequestStatus.InProgress)]
        [InlineData(RequestStatus.Resolved)]
        [InlineData(RequestStatus.Closed)]
        [Trait("Category", "Unit"), Trait("Req", "FR-010"), Trait("Priority", "Medium"), Trait("Type", "Positive")]
        public void Factory_ReturnsTheStateMatchingTheStatus(RequestStatus status)
        {
            Assert.Equal(status, RequestStateFactory.For(status).Status);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-010"), Trait("Priority", "Medium"), Trait("Type", "Negative")]
        public void Factory_UnknownStatus_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => RequestStateFactory.For((RequestStatus)99));
        }
    }
}
