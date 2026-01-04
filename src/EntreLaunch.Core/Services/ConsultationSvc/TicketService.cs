using Twilio.TwiML.Messaging;
using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.ConsultationDtos;
using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.Extensions;

namespace EntreLaunch.Services.ConsultationSvc
{
    public class TicketService(
        ILogger<MyOpportunityService> logger,
        IMapper mapper,
        PgDbContext pgDbContext,
        ILocalizationManager localizationManager,
        ICounselorService counselorService) : ITicketService
    {
        private readonly ILogger<MyOpportunityService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly PgDbContext _dbContext = pgDbContext;
        private readonly ICounselorService _counselorService = counselorService;
        private readonly ILocalizationManager _localizationManager = localizationManager;

        /// <inheritdoc />
        public async Task<GeneralResult> CreateTicket(TicketCreateDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogError("All data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("AllDataIsRequired"),
                        Data = null
                    };
                }

                if ((await _counselorService.IsCounselor(dto.CreatorId)).IsSuccess == false)
                {
                    _logger.LogError($"Counselor with this id {dto.CreatorId} do not have permission to open a ticket.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UnauthorizedSender"),
                        Data = null
                    };
                }

                var consultationType = await _dbContext.Consultations
                    .AnyAsync(c => c.Id == dto.ConsultationId && !c.IsDeleted && c.Type == ConsultationType.text);
                if (consultationType)
                {
                    _logger.LogError($"Can not open a tecket for this consultations {dto.ConsultationId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("ConsultationTypeNotAllowed"),
                        Data = null
                    };
                }

                var creator = await _dbContext.Consultations
                    .AnyAsync(c => c.CounselorId == dto.CreatorId && !c.IsDeleted);
                if (!creator)
                {
                    _logger.LogError($"Creator of ticket {dto.CreatorId} does not have permission to open a ticket.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("OnlyConsultationCreatorCanOpenTicket"),
                        Data = null
                    };
                }

                var consultationTicket = await _dbContext.ConsultationTickets
                    .AnyAsync(x => x.ConsultationId == dto.ConsultationId && !x.IsDeleted);
                if (consultationTicket)
                {
                    _logger.LogError($"Consultation with this id {dto.ConsultationId} already opened a ticket.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("ConsultationAlreadyHasTicket"),
                        Data = null
                    };
                }

                var ticketOpentMap = _mapper.Map<ConsultationTicket>(dto);
                ticketOpentMap.CreatedAt = DateTimeOffset.UtcNow;
                ticketOpentMap.Status = ConsultationTicketStatus.Open;
                ticketOpentMap.IsDeleted = false;
                await _dbContext.ConsultationTickets.AddAsync(ticketOpentMap);
                await _dbContext.SaveChangesAsync();

                _logger.LogError($"Ticket {ticketOpentMap.Id} opened successfully.");
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("TicketOpenedSuccessfully"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while opening new ticket.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("UnexpectedErrorWhileOpeningTicket"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> UpdateTicketStatus(ProcessTicketDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogError("All data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("AllDataIsRequired"),
                        Data = null
                    };
                }

                var ticket = _dbContext.ConsultationTickets.FirstOrDefault(o => o.Id == dto.Id && !o.IsDeleted);

                if (ticket == null)
                {
                    _logger.LogError($"No ticket found with Id {dto.Id}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("NoDataFound"),
                        Data = null
                    };
                }

                if (ticket.Status == dto.Status)
                {
                    _logger.LogError($"ticket with Id {dto.Id} is already {dto.Status}");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("TicketAlreadyInStatus"),
                        Data = null
                    };
                }

                ticket.Status = dto.Status;
                if (dto.Status == ConsultationTicketStatus.Closed)
                {
                    ticket.EndDate = DateTimeOffset.UtcNow;
                }

                ticket.UpdatedAt = DateTimeOffset.UtcNow;
                _dbContext.ConsultationTickets.Update(ticket);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("TicketUpdatedSuccessfully"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while processing ticket.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("UnexpectedErrorWhileProcessingTicket"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<TicketFullDetailsDto>>> GetAllTickets(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = GetFullConsultationTicketsQueryable();

                var pagedResult = await query
                    .AsNoTracking()
                    .ToPagedResultAsync(pagination, cancellationToken);

                if (!pagedResult.Items.Any())
                {
                    _logger.LogInformation("No tickets found.");
                    return new GeneralResult<PaginatedResult<TicketFullDetailsDto>>(false, _localizationManager.GetLocalizedString("NoTicketsFound"), null);
                }

                _logger.LogInformation("Retrieved tickets successfully.");
                return new GeneralResult<PaginatedResult<TicketFullDetailsDto>>(true, _localizationManager.GetLocalizedString("TicketsRetrievedSuccessfully"), pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving tickets.");
                return new GeneralResult<PaginatedResult<TicketFullDetailsDto>>(false, _localizationManager.GetLocalizedString("UnexpectedErrorWhileRetrievingTickets"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<TicketFullDetailsDto>> GetTicketById(int id)
        {
            try
            {
                var ticket = (await GetFullConsultationTicketsAsync(t => t.Id == id)).FirstOrDefault();
                if (ticket == null)
                {
                    _logger.LogInformation("No ticket found.");
                    return new GeneralResult<TicketFullDetailsDto>(false, _localizationManager.GetLocalizedString("NoTicketFound"), null);
                }

                _logger.LogInformation("Retrieved ticket successfully.");
                return new GeneralResult<TicketFullDetailsDto>(true, _localizationManager.GetLocalizedString("TicketsRetrievedSuccessfully"), ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving ticket.");
                return new GeneralResult<TicketFullDetailsDto>(false, _localizationManager.GetLocalizedString("UnexpectedErrorWhileRetrievingTicket"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<TicketFullDetailsDto>> GetTicketByConsultationId(int consultationId)
        {
            try
            {
                var ticket = await GetFullConsultationTicketsQueryable()
                    .FirstOrDefaultAsync(t => t.Consultation.Id == consultationId);

                if (ticket == null)
                {
                    _logger.LogInformation("No ticket for consultation ID {Id}.", consultationId);
                    return new GeneralResult<TicketFullDetailsDto>(false, _localizationManager.GetLocalizedString("NoTicketForThisConsultation"), null);
                }

                _logger.LogInformation("Retrieved ticket for consultation ID {Id}.", consultationId);
                return new GeneralResult<TicketFullDetailsDto>(true, _localizationManager.GetLocalizedString("TicketsRetrievedSuccessfully"), ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving ticket for consultation ID {Id}.", consultationId);
                return new GeneralResult<TicketFullDetailsDto>(false, _localizationManager.GetLocalizedString("UnexpectedErrorWhileRetrievingConsultationTicket"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> CreateTicketMessage(TicketMessageCreateDto dto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (dto == null)
                {
                    _logger.LogError("All data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("AllDataIsRequired"),
                        Data = null
                    };
                }

                // get ticket data
                var ticket = await _dbContext.ConsultationTickets
                    .Include(t => t.Consultation)
                    .ThenInclude(c => c.Counselor) // get counselor data
                    .FirstOrDefaultAsync(t => t.Id == dto.TicketId && !t.IsDeleted);

                if (ticket == null)
                {
                    _logger.LogError($"No ticket found with id {dto.TicketId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("TicketNotFound"),
                        Data = null
                    };
                }

                if(ticket.Status == ConsultationTicketStatus.Closed)
                {
                    _logger.LogError("Ticket is closed.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("TicketIsClosed"),
                        Data = null
                    };
                }

                // get user data
                var clientUserId = ticket.Consultation.ClientId;
                var counselorUserId = ticket.Consultation.Counselor.UserId;

                if (dto.SenderId != clientUserId &&
                    dto.SenderId != counselorUserId)
                {
                    _logger.LogError("Sender is not authorized for this ticket.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UnauthorizedToSendMessage"),
                        Data = null
                    };
                }

                var messageSender = await _dbContext.Users.Where(u => u.Id == dto.SenderId && !u.IsDeleted).FirstOrDefaultAsync();
                if (messageSender == null)
                {
                    _logger.LogError($"No user found with this id: {dto.SenderId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserNotFound"),
                        Data = null
                    };
                }

                var messageMap = _mapper.Map<ConsultationTicketMessage>(dto);
                messageMap.CreatedAt = DateTimeOffset.UtcNow;
                messageMap.SendTime = DateTimeOffset.UtcNow;
                messageMap.IsDeleted = false;


                if (dto.SenderId == clientUserId)
                {
                    messageMap.IsClientMessage = true;
                }

                await _dbContext.ConsultationTicketMessages.AddAsync(messageMap);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogError($"Ticket message with id {messageMap.Id} sent successfully.");
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("TicketMessageSentSuccessfully"),
                    Data = _mapper.Map<TicketMessageDetailsDto>(messageMap)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending ticket message.");
                await transaction.RollbackAsync();
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("UnexpectedErrorWhileSendingTicketMessage"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> UpdateTicketMessage(int id, TicketMessageUpdateDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogError("All data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("AllDataIsRequired"),
                        Data = null
                    };
                }

                var message = await _dbContext.ConsultationTicketMessages
                    .Include(m => m.Ticket) 
                    .Where(u => u.Id == id && !u.IsDeleted).FirstOrDefaultAsync();
                if (message == null)
                {
                    _logger.LogError($"No message found with this id: {id}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("MessageNotFound"),
                        Data = null
                    };
                }

                if (message.Ticket.Status == ConsultationTicketStatus.Closed)
                {
                    _logger.LogError("Ticket is closed.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("TicketIsClosed"),
                        Data = null
                    };
                }

                _mapper.Map(dto, message);
                message.UpdatedAt = DateTimeOffset.UtcNow;
                _dbContext.ConsultationTicketMessages.Update(message);
                await _dbContext.SaveChangesAsync();

                _logger.LogError($"Ticket message with id {message.Id} updated successfully.");
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("TicketMessageEditedSuccessfully"),
                    Data = _mapper.Map<TicketMessageDetailsDto>(message)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while editing ticket message.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("UnexpectedErrorWhileEditingTicketMessage"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> DeleteTicketMessage(int id)
        {
            try
            {
                var message = _dbContext.ConsultationTicketMessages.FirstOrDefault(u => u.Id == id && !u.IsDeleted);
                if (message == null)
                {
                    _logger.LogError($"No message found with this id: {id}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("MessageNotFound"),
                        Data = null
                    };
                }

                if (message.Ticket.Status == ConsultationTicketStatus.Closed)
                {
                    _logger.LogError("Ticket is closed.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("TicketIsClosed"),
                        Data = null
                    };
                }

                message.IsDeleted = true;
                message.DeletedAt = DateTimeOffset.UtcNow;
                _dbContext.ConsultationTicketMessages.Update(message);
                await _dbContext.SaveChangesAsync();

                _logger.LogError($"Ticket message with id {message.Id} deleted successfully.");
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("TicketMessageDeletedSuccessfully"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting ticket message.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("UnexpectedErrorWhileDeletingTicketMessage"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<TicketMessageDetailsDto>>> GetTicketMessages(int ticketId, PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.ConsultationTicketMessages
                    .AsNoTracking()
                    .Where(cm => cm.TicketId == ticketId && !cm.IsDeleted)
                    .Select(cm => new TicketMessageDetailsDto
                    {
                        Id = cm.Id,
                        SenderId = cm.SenderId,
                        Content = cm.Content,
                        IsClientMessage = cm.IsClientMessage,
                    });

                var pagedResult = await query.ToPagedResultAsync(pagination, cancellationToken);

                if (!pagedResult.Items.Any())
                {
                    _logger.LogInformation("No ticket messages found for ticket ID {TicketId}.", ticketId);
                    return new GeneralResult<PaginatedResult<TicketMessageDetailsDto>>(false, _localizationManager.GetLocalizedString("NoMessagesFound"), null);
                }

                _logger.LogInformation("Retrieved messages for ticket ID {TicketId}.", ticketId);
                return new GeneralResult<PaginatedResult<TicketMessageDetailsDto>>(true, _localizationManager.GetLocalizedString("MessagesFound"), pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ticket messages for ticket ID {TicketId}.", ticketId);
                return new GeneralResult<PaginatedResult<TicketMessageDetailsDto>>(false, _localizationManager.GetLocalizedString("UnexpectedErrorWhileGettingMessages"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> CreateTicketAttachment(TicketAttachmentCreateDto dto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (dto == null)
                {
                    _logger.LogError("All data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("AllDataIsRequired"),
                        Data = null
                    };
                }

                // get ticket data
                var ticket = await _dbContext.ConsultationTickets
                    .Include(t => t.Consultation)
                    .ThenInclude(c => c.Counselor) // get counselor data
                    .FirstOrDefaultAsync(t => t.Id == dto.TicketId && !t.IsDeleted);

                if (ticket == null)
                {
                    _logger.LogError($"No ticket found with id {dto.TicketId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("NoTicketFound"),
                        Data = null
                    };
                }

                if (ticket.Status == ConsultationTicketStatus.Closed)
                {
                    _logger.LogError("Ticket is closed.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("TicketIsClosed"),
                        Data = null
                    };
                }

                // get user data
                var clientUserId = ticket.Consultation.ClientId;
                var counselorUserId = ticket.Consultation.Counselor.UserId;

                if (dto.SenderId != clientUserId &&
                    dto.SenderId != counselorUserId)
                {
                    _logger.LogError("Sender is not authorized for this ticket.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UnauthorizedToSendAttachment"),
                        Data = null
                    };
                }


                var attachmentSender = _dbContext.Users.FirstOrDefault(u => u.Id == dto.SenderId && !u.IsDeleted);
                if (attachmentSender == null)
                {
                    _logger.LogError($"No user found with this id: {dto.SenderId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserNotFound"),
                        Data = null
                    };
                }

                var attachmentMap = _mapper.Map<ConsultationTicketAttachment>(dto);
                attachmentMap.CreatedAt = DateTimeOffset.UtcNow;
                attachmentMap.SendTime = DateTimeOffset.UtcNow;
                attachmentMap.IsDeleted = false;


                if (dto.SenderId == clientUserId)
                {
                    attachmentMap.IsClientMessage = true;
                }

                await _dbContext.ConsultationTicketAttachments.AddAsync(attachmentMap);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Ticket attachment with id {attachmentMap.Id} sent successfully.");
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("TicketAttachmentSentSuccessfully"),
                    Data = _mapper.Map<TicketAttachmentDetailsDto>(attachmentMap)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending ticket attachment.");
                await transaction.RollbackAsync();
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("UnexpectedErrorWhileSendingTicketAttachment"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> DeleteTicketAttachment(int id, string userId)
        {
            try
            {
                var user = await _dbContext.Users.AsNoTracking().AnyAsync(u => u.Id == userId && !u.IsDeleted);
                if (user == false)
                {
                    _logger.LogError($"No user found with this id: {userId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserNotFound"),
                        Data = null
                    };
                }

                var attachment = await _dbContext.ConsultationTicketAttachments
                    .Where(u => u.Id == id && !u.IsDeleted && u.SenderId == userId).FirstOrDefaultAsync();
                if (attachment == null)
                {
                    _logger.LogError($"No attachment found with this id: {id}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("AttachmentNotFound"),
                        Data = null
                    };
                }

                if (attachment.Ticket.Status == ConsultationTicketStatus.Closed)
                {
                    _logger.LogError("Ticket is closed.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("TicketIsClosed"),
                        Data = null
                    };
                }

                attachment.IsDeleted = true;
                attachment.DeletedAt = DateTimeOffset.UtcNow;
                _dbContext.ConsultationTicketAttachments.Update(attachment);
                await _dbContext.SaveChangesAsync();

                _logger.LogError($"Ticket attachment with id {attachment.Id} deleted successfully.");
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("TicketAttachmentDeletedSuccessfully"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting ticket attachment.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("UnexpectedErrorWhileDeletingTicketAttachment"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<TicketFullDetailsDto>>> GetTicketsByCounselor(int counselorId, PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                if (counselorId <= 0)
                {
                    _logger.LogWarning("Invalid counselor ID provided.");
                    return new GeneralResult<PaginatedResult<TicketFullDetailsDto>>(
                        false, _localizationManager.GetLocalizedString("InvalidCounselorId"), null);
                }

                var now = DateTimeOffset.UtcNow;

                var query = GetFullConsultationTicketsQuery()
                    .Where(t => t.CreatorId == counselorId)
                    .Select(t => new TicketFullDetailsDto
                    {
                        Id = t.Id,
                        Creator = new CounselorData
                        {
                            Id = t.Creator.Id,
                            FirstName = t.Creator.User.FirstName,
                            LastName = t.Creator.User.LastName,
                            Email = t.Creator.User.Email
                        },
                        Consultation = new ConsultationAllData
                        {
                            Id = t.Consultation.Id,
                            Type = t.Consultation.Type,
                            Description = t.Consultation.Description,
                            Status = t.Consultation.Status,
                            counselorData = null,
                            ConsultationTimeDate = t.Consultation.ConsultationTime != null
                                ? t.Consultation.ConsultationTime.DateTimeSlot
                                : now,
                            customerData = new CustomerData
                            {
                                Id = t.Consultation.Client.Id,
                                FirstName = t.Consultation.Client.FirstName ?? "",
                                LastName = t.Consultation.Client.LastName ?? "",
                                NationalId = t.Consultation.Client.NationalId ?? 0,
                                Specialization = t.Consultation.Client.Specialization ?? "",
                                CountryCode = t.Consultation.Client.CountryCode,
                                Email = t.Consultation.Client.Email ?? "",
                                DateOfBirth = t.Consultation.Client.DOB ?? now,
                            }
                        },
                        Status = t.Status ?? ConsultationTicketStatus.Open,
                        UpdatedAt = t.UpdatedAt,
                        Textmessages = t.TicketMessages
                            .Where(m => !string.IsNullOrEmpty(m.Content))
                            .Select(m => new TicketMessages
                            {
                                Id = m.Id,
                                SenderId = m.SenderId,
                                SenderName = $"{m.Sender.FirstName} {m.Sender.LastName}",
                                Content = m.Content,
                                IsClientMessage = m.IsClientMessage,
                                SendTime = m.CreatedAt ?? now
                            }).FirstOrDefault(),
                        Media = t.TicketAttachments
                            .Select(m => new MediaMessages
                            {
                                Id = m.Id,
                                Url = m.Url,
                                MediaType = "Media",
                                SenderId = m.SenderId,
                                SenderName = $"{m.Sender.FirstName} {m.Sender.LastName}",
                                IsClientMessage = m.IsClientMessage,
                                SendTime = m.CreatedAt ?? now
                            }).FirstOrDefault()
                    });

                var pagedResult = await query.ToPagedResultAsync(pagination, cancellationToken);

                if (!pagedResult.Items.Any())
                {
                    _logger.LogInformation("No tickets found for counselor with ID {CounselorId}.", counselorId);
                    return new GeneralResult<PaginatedResult<TicketFullDetailsDto>>(
                        false, _localizationManager.GetLocalizedString("NoTicketsFoundForCounselor"), null);
                }

                _logger.LogInformation("Retrieved tickets for counselor ID {CounselorId}.", counselorId);
                return new GeneralResult<PaginatedResult<TicketFullDetailsDto>>(
                    true, _localizationManager.GetLocalizedString("TicketsRetrievedSuccessfully"), pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving tickets by counselor.");
                return new GeneralResult<PaginatedResult<TicketFullDetailsDto>>(
                    false, _localizationManager.GetLocalizedString("UnexpectedErrorWhileRetrievingTickets"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<TicketFullDetailsDto>>> GetOpenTicketsForConsultation(int consultationId)
        {
            try
            {
                var openTickets = await GetFullConsultationTicketsAsync(t =>
                    t.ConsultationId == consultationId &&
                    t.Status == ConsultationTicketStatus.Open &&
                    !t.IsDeleted);

                if (!openTickets.Any())
                {
                    _logger.LogInformation("No open tickets found for consultation ID {ConsultationId}.", consultationId);
                    return new GeneralResult<List<TicketFullDetailsDto>>(false, _localizationManager.GetLocalizedString("NoOpenTicketsForConsultation"), null);
                }

                _logger.LogInformation("Retrieved open tickets for consultation ID {ConsultationId}.", consultationId);
                return new GeneralResult<List<TicketFullDetailsDto>>(true, _localizationManager.GetLocalizedString("OpenTicketsRetrievedSuccessfully"), openTickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving open tickets for consultation ID {ConsultationId}.", consultationId);
                return new GeneralResult<List<TicketFullDetailsDto>>(false, _localizationManager.GetLocalizedString("UnexpectedErrorWhileRetrievingOpenTickets"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<bool>> CanUserAccessTicket(int ticketId, string userId)
        {
            try
            {
                var ticket = await _dbContext.ConsultationTickets
                    .Include(t => t.Consultation)
                    .ThenInclude(c => c.Counselor)
                    .FirstOrDefaultAsync(t => t.Id == ticketId && !t.IsDeleted);

                if (ticket == null)
                {
                    _logger.LogWarning("No ticket found with ID {TicketId}.", ticketId);
                    return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("TicketNotFound"), false);
                }

                var consultation = ticket.Consultation;
                if (consultation == null)
                {
                    _logger.LogWarning("Consultation not found for ticket ID {TicketId}.", ticketId);
                    return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("ConsultationNotFound"), false);
                }

                var isClient = consultation.ClientId == userId;
                var isCounselor = consultation.Counselor?.UserId == userId;

                var hasAccess = isClient || isCounselor;

                return new GeneralResult<bool>(true, hasAccess
                    ? _localizationManager.GetLocalizedString("AccessGranted")
                    : _localizationManager.GetLocalizedString("AccessDenied"), hasAccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while checking access for ticket ID {TicketId}.", ticketId);
                return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("UnexpectedErrorCheckingTicketAccess"), false);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<TicketAttachmentDetailsDto>>> GetTicketAttachments(int ticketId, PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.ConsultationTicketAttachments
                    .AsNoTracking()
                    .Where(cm => cm.TicketId == ticketId && !cm.IsDeleted)
                    .Select(cm => new TicketAttachmentDetailsDto
                    {
                        Id = cm.Id,
                        Url = cm.Url,
                        SenderId = cm.SenderId,
                        CreatedAt = cm.CreatedAt ?? DateTimeOffset.UtcNow
                    });

                var pagedResult = await query.ToPagedResultAsync(pagination, cancellationToken);

                if (!pagedResult.Items.Any())
                {
                    _logger.LogWarning("No attachments found for ticket ID {TicketId}.", ticketId);
                    return new GeneralResult<PaginatedResult<TicketAttachmentDetailsDto>>(false, _localizationManager.GetLocalizedString("NoAttachmentsFound"), null);
                }

                _logger.LogInformation("Retrieved attachments for ticket ID {TicketId}.", ticketId);
                return new GeneralResult<PaginatedResult<TicketAttachmentDetailsDto>>(true, _localizationManager.GetLocalizedString("AttachmentsFound"), pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving attachments for ticket ID {TicketId}.", ticketId);
                return new GeneralResult<PaginatedResult<TicketAttachmentDetailsDto>>(false, _localizationManager.GetLocalizedString("UnexpectedErrorWhileGettingAttachments"), null);
            }
        }

        private async Task<List<TicketFullDetailsDto>> GetFullConsultationTicketsAsync(Func<ConsultationTicket, bool>? predicate = null)
        {
            var now = DateTimeOffset.UtcNow;

            var tickets = await _dbContext.ConsultationTickets
                .AsNoTracking()
                .Where(t => !t.IsDeleted)
                .Include(t => t.Creator).ThenInclude(c => c.User)
                .Include(t => t.Consultation).ThenInclude(c => c.Client)
                .Include(t => t.Consultation).ThenInclude(c => c.ConsultationTime)
                .Include(t => t.TicketMessages).ThenInclude(m => m.Sender)
                .Include(t => t.TicketAttachments).ThenInclude(a => a.Sender)
                .ToListAsync();

            if (predicate != null)
                tickets = tickets.Where(predicate).ToList();

            return tickets.Select(t => new TicketFullDetailsDto
            {
                Id = t.Id,
                Creator = new CounselorData
                {
                    Id = t.Creator.Id,
                    FirstName = t.Creator.User.FirstName,
                    LastName = t.Creator.User.LastName,
                    Email = t.Creator.User.Email
                },
                Consultation = new ConsultationAllData
                {
                    Id = t.Consultation.Id,
                    Type = t.Consultation.Type,
                    Description = t.Consultation.Description,
                    Status = t.Consultation.Status,
                    counselorData = null,
                    ConsultationTimeDate = t.Consultation.ConsultationTime?.DateTimeSlot ?? now,
                    customerData = new CustomerData
                    {
                        Id = t.Consultation.Client.Id,
                        FirstName = t.Consultation.Client.FirstName ?? "",
                        LastName = t.Consultation.Client.LastName ?? "",
                        NationalId = t.Consultation.Client.NationalId ?? 0,
                        Specialization = t.Consultation.Client.Specialization ?? "",
                        CountryCode = t.Consultation.Client.CountryCode,
                        Email = t.Consultation.Client.Email ?? "",
                        DateOfBirth = t.Consultation.Client.DOB ?? now,
                    }
                },
                Status = t.Status ?? ConsultationTicketStatus.Open,
                UpdatedAt = t.UpdatedAt,
                Textmessages = t.TicketMessages?
                    .Where(m => !string.IsNullOrEmpty(m.Content))
                    .Select(m => new TicketMessages
                    {
                        Id = m.Id,
                        SenderId = m.SenderId,
                        SenderName = $"{m.Sender.FirstName} {m.Sender.LastName}",
                        Content = m.Content,
                        IsClientMessage = m.IsClientMessage,
                        SendTime = m.CreatedAt ?? now
                    }).FirstOrDefault(),
                Media = t.TicketAttachments?
                    .Select(m => new MediaMessages
                    {
                        Id = m.Id,
                        Url = m.Url,
                        MediaType = "Media",
                        SenderId = m.SenderId,
                        SenderName = $"{m.Sender.FirstName} {m.Sender.LastName}",
                        IsClientMessage = m.IsClientMessage,
                        SendTime = m.CreatedAt ?? now
                    }).FirstOrDefault()
            }).ToList();
        }
        private IQueryable<ConsultationTicket> GetFullConsultationTicketsQuery()
        {
            return _dbContext.ConsultationTickets
                .AsNoTracking()
                .Where(t => !t.IsDeleted)
                .Include(t => t.Creator).ThenInclude(c => c.User)
                .Include(t => t.Consultation).ThenInclude(c => c.Client)
                .Include(t => t.Consultation).ThenInclude(c => c.ConsultationTime)
                .Include(t => t.TicketMessages).ThenInclude(m => m.Sender)
                .Include(t => t.TicketAttachments).ThenInclude(a => a.Sender);
        }
        private IQueryable<TicketFullDetailsDto> GetFullConsultationTicketsQueryable()
        {
            var now = DateTimeOffset.UtcNow;

            return _dbContext.ConsultationTickets
                .Where(t => !t.IsDeleted)
                .Include(t => t.Creator).ThenInclude(c => c.User)
                .Include(t => t.Consultation).ThenInclude(c => c.Client)
                .Include(t => t.Consultation).ThenInclude(c => c.ConsultationTime)
                .Include(t => t.TicketMessages).ThenInclude(m => m.Sender)
                .Include(t => t.TicketAttachments).ThenInclude(a => a.Sender)
                .Select(t => new TicketFullDetailsDto
                {
                    Id = t.Id,
                    Creator = new CounselorData
                    {
                        Id = t.Creator.Id,
                        FirstName = t.Creator.User.FirstName,
                        LastName = t.Creator.User.LastName,
                        Email = t.Creator.User.Email
                    },
                    Consultation = new ConsultationAllData
                    {
                        Id = t.Consultation.Id,
                        Type = t.Consultation.Type,
                        Description = t.Consultation.Description,
                        Status = t.Consultation.Status,
                        counselorData = null,
                        ConsultationTimeDate = t.Consultation.ConsultationTime != null ? t.Consultation.ConsultationTime.DateTimeSlot : now,
                        customerData = new CustomerData
                        {
                            Id = t.Consultation.Client.Id,
                            FirstName = t.Consultation.Client.FirstName ?? "",
                            LastName = t.Consultation.Client.LastName ?? "",
                            NationalId = t.Consultation.Client.NationalId ?? 0,
                            Specialization = t.Consultation.Client.Specialization ?? "",
                            CountryCode = t.Consultation.Client.CountryCode,
                            Email = t.Consultation.Client.Email ?? "",
                            DateOfBirth = t.Consultation.Client.DOB ?? now
                        }
                    },
                    Status = t.Status ?? ConsultationTicketStatus.Open,
                    UpdatedAt = t.UpdatedAt,
                    Textmessages = t.TicketMessages!
                        .Where(m => !string.IsNullOrEmpty(m.Content))
                        .Select(m => new TicketMessages
                        {
                            Id = m.Id,
                            SenderId = m.SenderId,
                            SenderName = $"{m.Sender.FirstName} {m.Sender.LastName}",
                            Content = m.Content,
                            IsClientMessage = m.IsClientMessage,
                            SendTime = m.CreatedAt ?? now
                        }).FirstOrDefault(),
                    Media = t.TicketAttachments!
                        .Select(m => new MediaMessages
                        {
                            Id = m.Id,
                            Url = m.Url,
                            MediaType = "Media",
                            SenderId = m.SenderId,
                            SenderName = $"{m.Sender.FirstName} {m.Sender.LastName}",
                            IsClientMessage = m.IsClientMessage,
                            SendTime = m.CreatedAt ?? now
                        }).FirstOrDefault()
                });
        }
    }
}
