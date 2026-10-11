namespace CivicConnect.ViewModels
{
    /// <summary>OWNER: Enrique de Sousa. Entity -> ViewModel mapping, kept out of controllers so controllers stay thin.</summary>
    public static class RequestMapper
    {
        public static RequestListItemViewModel ToListItem(ServiceRequest request)
        {
            // TODO: copy Id, ReferenceNo, Status, SubmittedAt; CategoryName = request.Category?.Name ?? ""; AssignedStaffName = request.AssignedStaff?.DisplayName.
            throw new NotImplementedException();
        }

        public static RequestDetailsViewModel ToDetails(ServiceRequest request, AvailableActions actions, IReadOnlyList<User> assignableStaff)
        {
            // TODO: map scalar fields (RequesterName = request.Requester?.DisplayName), RowVersion, Actions = actions;
            // History = request.StatusHistory ordered by ChangedAt -> HistoryItemViewModel (ChangedByName = ChangedBy?.DisplayName);
            // Comments = request.Comments ordered by CreatedAt -> CommentItemViewModel (AuthorName = Author?.DisplayName);
            // AssignableStaff = new SelectList(assignableStaff, nameof(User.Id), nameof(User.DisplayName)).
            throw new NotImplementedException();
        }

        public static PagedResult<RequestListItemViewModel> ToPagedListItems(PagedResult<ServiceRequest> page)
        {
            // TODO: new PagedResult<RequestListItemViewModel> { Items = page.Items.Select(ToListItem).ToList(), TotalCount, Page, PageSize }.
            throw new NotImplementedException();
        }
    }
}
