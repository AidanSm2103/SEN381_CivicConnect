namespace CivicConnect.ViewModels
{
    /// <summary>OWNER: Enrique de Sousa. Entity -> ViewModel mapping, kept out of controllers so controllers stay thin.</summary>
    public static class RequestMapper
    {
        public static RequestListItemViewModel ToListItem(ServiceRequest request)
        {
            return new RequestListItemViewModel
            {
                Id = request.Id,
                ReferenceNo = request.ReferenceNo,
                CategoryName = request.Category?.Name ?? string.Empty,
                Status = request.Status,
                SubmittedAt = request.SubmittedAt,
                AssignedStaffName = request.AssignedStaff?.DisplayName
            };
        }

        public static RequestDetailsViewModel ToDetails(ServiceRequest request, AvailableActions actions, IReadOnlyList<User> assignableStaff)
        {
            return new RequestDetailsViewModel
            {
                Id = request.Id,
                ReferenceNo = request.ReferenceNo,
                CategoryName = request.Category?.Name ?? string.Empty,
                Description = request.Description,
                Status = request.Status,
                SubmittedAt = request.SubmittedAt,
                RequesterName = request.Requester?.DisplayName ?? string.Empty,
                AssignedStaffName = request.AssignedStaff?.DisplayName,
                RowVersion = request.RowVersion,
                Actions = actions,
                History = request.StatusHistory
                    .OrderBy(h => h.ChangedAt).ThenBy(h => h.Id)
                    .Select(h => new HistoryItemViewModel
                    {
                        FromStatus = h.FromStatus,
                        ToStatus = h.ToStatus,
                        ChangedByName = h.ChangedBy?.DisplayName ?? string.Empty,
                        ChangedAt = h.ChangedAt,
                        Note = h.Note
                    }).ToList(),
                Comments = request.Comments
                    .OrderBy(c => c.CreatedAt).ThenBy(c => c.Id)
                    .Select(c => new CommentItemViewModel
                    {
                        Kind = c.Kind,
                        Body = c.Body,
                        AuthorName = c.Author?.DisplayName ?? string.Empty,
                        CreatedAt = c.CreatedAt
                    }).ToList(),
                AssignableStaff = new SelectList(assignableStaff, nameof(User.Id), nameof(User.DisplayName))
            };
        }

        public static PagedResult<RequestListItemViewModel> ToPagedListItems(PagedResult<ServiceRequest> page)
        {
            return new PagedResult<RequestListItemViewModel>
            {
                Items = page.Items.Select(ToListItem).ToList(),
                TotalCount = page.TotalCount,
                Page = page.Page,
                PageSize = page.PageSize
            };
        }
    }
}
