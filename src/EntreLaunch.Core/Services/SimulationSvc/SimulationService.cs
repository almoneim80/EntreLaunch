namespace EntreLaunch.Services
{
    public class SimulationService : ISimulationService
    {
        private readonly PgDbContext _dbContext;
        private readonly ILogger<SimulationService> _logger;
        public SimulationService(
            PgDbContext dbContext,
            ILogger<SimulationService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> CreateSimulationAsync(SimulationCreateDto dto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                /***************************** create simulation *****************************/
                if (string.IsNullOrWhiteSpace(dto.projectDto.ProjectField) ||
                    string.IsNullOrWhiteSpace(dto.projectDto.ProjectType) ||
                    string.IsNullOrWhiteSpace(dto.projectDto.UserId))
                {
                    _logger.LogError("Project data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Project data is required",
                        Data = null
                    };
                }

                var user = await _dbContext.Users
                    .AsNoTracking().AnyAsync(u => u.Id == dto.projectDto.UserId && !u.IsDeleted);
                if (!user)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User not found",
                        Data = null
                    };
                }

                var simulation = new Simulation
                {
                    UserId = dto.projectDto.UserId,
                    ProjectField = dto.projectDto.ProjectField,
                    ProjectType = dto.projectDto.ProjectType,
                    CreatedAt = DateTimeOffset.UtcNow
                };

                /***************************** Adding idea power *****************************/
                int positiveFactors = 0;
                int negativeFactors = 0;

                if (dto.IdeaPowerhDto != null && dto.IdeaPowerhDto.Any())
                {
                    var ideaStrengthEntities = new List<SimulationIdeaPower>();

                    foreach (var item in dto.IdeaPowerhDto)
                    {
                        if (item.FactorData == null || !item.FactorData.Any())
                        {
                            return new GeneralResult
                            {
                                IsSuccess = false,
                                Message = $"Factor data for strength category '{item.CategoryName}' is required.",
                                Data = null
                            };
                        }

                        if (item.Total == 0)
                        {
                            return new GeneralResult
                            {
                                IsSuccess = false,
                                Message = "Total is required.",
                                Data = null
                            };
                        }

                        if (item.CategoryName == null)
                        {
                            return new GeneralResult
                            {
                                IsSuccess = false,
                                Message = "Category name is required.",
                                Data = null
                            };
                        }

                        foreach (var factor in item.FactorData)
                        {
                            if (string.IsNullOrWhiteSpace(factor.StrengthFactor))
                            {
                                return new GeneralResult
                                {
                                    IsSuccess = false,
                                    Message = "Strength factor is required.",
                                    Data = null
                                };
                            }

                            if (!factor.FactorScore.HasValue || factor.FactorScore.Value == 0)
                            {
                                return new GeneralResult
                                {
                                    IsSuccess = false,
                                    Message = "Factor score must be greater than zero.",
                                    Data = null
                                };
                            }

                            ideaStrengthEntities.Add(new SimulationIdeaPower
                            {
                                Simulation = simulation,
                                CategoryType = item.CategoryType,
                                CategoryName = item.CategoryName,
                                StrengthFactor = factor.StrengthFactor,
                                FactorScore = factor.FactorScore.Value,
                                CreatedAt = DateTimeOffset.UtcNow
                            });
                        }

                        if (item.CategoryType == Category.Positive)
                        {
                            positiveFactors = positiveFactors + item.Total;
                        }
                        else
                        {
                            negativeFactors = negativeFactors + item.Total;
                        }
                    }

                    await _dbContext.SimulationIdeaPowers.AddRangeAsync(ideaStrengthEntities);
                }
                else
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Idea strength data is required",
                        Data = null
                    };
                }

                /***************************** Adding business plan *****************************/
                SimulationBusinessPlan? businessPlan = null;
                if (dto.BusinessPlanDto != null &&
                    (!string.IsNullOrWhiteSpace(dto.BusinessPlanDto.BusinessPlanFileUrl) ||
                     (dto.BusinessPlanDto.BusinessPartners != null && dto.BusinessPlanDto.BusinessPartners.Any()) ||
                     (dto.BusinessPlanDto.ProjectActivities != null && dto.BusinessPlanDto.ProjectActivities.Any()) ||
                     (dto.BusinessPlanDto.ValueProposition != null && dto.BusinessPlanDto.ValueProposition.Any()) ||
                     (dto.BusinessPlanDto.CustomerRelationships != null && dto.BusinessPlanDto.CustomerRelationships.Any()) ||
                     (dto.BusinessPlanDto.CustomerSegments != null && dto.BusinessPlanDto.CustomerSegments.Any()) ||
                     (dto.BusinessPlanDto.RequiredResources != null && dto.BusinessPlanDto.RequiredResources.Any()) ||
                     (dto.BusinessPlanDto.DistributionChannels != null && dto.BusinessPlanDto.DistributionChannels.Any()) ||
                     (dto.BusinessPlanDto.RevenueStreams != null && dto.BusinessPlanDto.RevenueStreams.Any()) ||
                     (dto.BusinessPlanDto.CostStructure != null && dto.BusinessPlanDto.CostStructure.Any())))
                {
                    businessPlan = new SimulationBusinessPlan
                    {
                        Simulation = simulation,
                        BusinessPlanFileUrl = dto.BusinessPlanDto.BusinessPlanFileUrl,
                        BusinessPartners = dto.BusinessPlanDto.BusinessPartners,
                        ProjectActivities = dto.BusinessPlanDto.ProjectActivities,
                        ValueProposition = dto.BusinessPlanDto.ValueProposition,
                        CustomerRelationships = dto.BusinessPlanDto.CustomerRelationships,
                        CustomerSegments = dto.BusinessPlanDto.CustomerSegments,
                        RequiredResources = dto.BusinessPlanDto.RequiredResources,
                        DistributionChannels = dto.BusinessPlanDto.DistributionChannels,
                        RevenueStreams = dto.BusinessPlanDto.RevenueStreams,
                        CostStructure = dto.BusinessPlanDto.CostStructure,
                        CreatedAt = DateTimeOffset.UtcNow
                    };
                    await _dbContext.SimulationBusinessPlans.AddAsync(businessPlan);
                }
                else
                {
                    return new GeneralResult { IsSuccess = false, Message = "Business plan is required", Data = null };
                }

                /***************************** Adding feasibility study *****************************/
                if (dto.FeasibilityStudyDto != null &&
                            (dto.FeasibilityStudyDto.CapitalMin.HasValue ||
                            dto.FeasibilityStudyDto.CapitalMax.HasValue ||
                             dto.FeasibilityStudyDto.InterestRate.HasValue ||
                             dto.FeasibilityStudyDto.MarketingCost.HasValue ||
                             dto.FeasibilityStudyDto.RentCost.HasValue ||
                             dto.FeasibilityStudyDto.DecorationCost > 0 ||
                             dto.FeasibilityStudyDto.EquipmentCost.HasValue ||
                             dto.FeasibilityStudyDto.GovFees.HasValue ||
                             dto.FeasibilityStudyDto.InventoryCost.HasValue))
                {
                    if (dto.FeasibilityStudyDto.CapitalMax <= dto.FeasibilityStudyDto.CapitalMin)
                    {
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = "Capital max must be greater than capital min.",
                            Data = null
                        };
                    }

                    if (dto.FeasibilityStudyDto.IsInterest.GetValueOrDefault() &&
                        dto.FeasibilityStudyDto.InterestRate <= 0)
                    {
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = "Interest rate must be greater than zero.",
                            Data = null
                        };
                    }

                    if (dto.FeasibilityStudyDto.MarketingCost > dto.FeasibilityStudyDto.CapitalMax ||
                        dto.FeasibilityStudyDto.MarketingCost < dto.FeasibilityStudyDto.CapitalMin)
                    {
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = "Marketing cost must be less than capital max, and greater than capital min.",
                            Data = null
                        };
                    }

                    if ((dto.FeasibilityStudyDto.MarketingCost +
                        dto.FeasibilityStudyDto.RentCost +
                        dto.FeasibilityStudyDto.DecorationCost +
                        dto.FeasibilityStudyDto.EquipmentCost +
                        dto.FeasibilityStudyDto.InventoryCost) >= dto.FeasibilityStudyDto.CapitalMax)
                    {
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = "Total cost must be less than capital max, and greater than capital min.",
                            Data = null
                        };
                    }

                    var feasibilityStudy = new SimulationFeasibilityStudy
                    {
                        Simulation = simulation,
                        ProjectName = dto.FeasibilityStudyDto.ProjectName,
                        MinCapital = dto.FeasibilityStudyDto.CapitalMin,
                        MaxCapital = dto.FeasibilityStudyDto.CapitalMax,
                        InterestRate = dto.FeasibilityStudyDto.InterestRate ?? 0,
                        MarketingCost = dto.FeasibilityStudyDto.MarketingCost,
                        RentCost = dto.FeasibilityStudyDto.RentCost,
                        DecorationCost = dto.FeasibilityStudyDto.DecorationCost ?? 0,
                        EquipmentCost = dto.FeasibilityStudyDto.EquipmentCost,
                        GovFees = dto.FeasibilityStudyDto.GovFees,
                        InventoryCost = dto.FeasibilityStudyDto.InventoryCost,
                        CreatedAt = DateTimeOffset.UtcNow
                    };

                    await _dbContext.SimulationFeasibilityStudies.AddAsync(feasibilityStudy);
                }
                else
                {
                    return new GeneralResult { IsSuccess = false, Message = "Feasibility study data is required", Data = null };
                }

                /***************************** Adding Purchases *****************************/
                if (dto.PurchaseDto != null)
                {
                    if (string.IsNullOrWhiteSpace(dto.PurchaseDto.Description))
                    {
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = "Purchase description is required",
                            Data = null
                        };
                    }

                    if (dto.PurchaseDto.Products == null || !dto.PurchaseDto.Products.Any())
                    {
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = "At least one product is required in a purchase",
                            Data = null
                        };
                    }

                    double totalCost = 0;
                    foreach (var product in dto.PurchaseDto.Products)
                    {
                        if (string.IsNullOrWhiteSpace(product.ItemName) || product.ItemCost <= 0)
                        {
                            return new GeneralResult
                            {
                                IsSuccess = false,
                                Message = "Each product must have a valid name and cost",
                                Data = null
                            };
                        }

                        totalCost += product.ItemCost;
                    }

                    if (totalCost >= dto.FeasibilityStudyDto.InventoryCost)
                    {
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = "The total procurement cost must be less than or " +
                            "equal to the cost of the goods identified in the feasibility study.",
                            Data = null
                        };
                    }

                    simulation.TotalPurchaseValue = totalCost;

                    var purchaseEntities = dto.PurchaseDto.Products
                            .Select(product => new SimulationPurchase
                            {
                                Simulation = simulation,
                                ItemName = product.ItemName,
                                ItemCost = product.ItemCost,
                                Description = dto.PurchaseDto.Description,
                                CreatedAt = DateTimeOffset.UtcNow
                            }).ToList();

                    await _dbContext.SimulationPurchases.AddRangeAsync(purchaseEntities);
                }

                /***************************** Adding Marketing Data *****************************/
                List<SimulationMarketing>? marketingEntities = null;
                if (dto.MarketingDto != null && dto.MarketingDto.Any())
                {
                    marketingEntities = new List<SimulationMarketing>();

                    foreach (var marketingItem in dto.MarketingDto)
                    {
                        if (string.IsNullOrWhiteSpace(marketingItem.ProductName) ||
                            marketingItem.UnitPrice <= 0 ||
                            marketingItem.Quantity <= 0)
                        {
                            return new GeneralResult
                            {
                                IsSuccess = false,
                                Message = "All marketing data is required",
                                Data = null
                            };
                        }

                        var marketingEntity = new SimulationMarketing
                        {
                            Simulation = simulation,
                            ProductName = marketingItem.ProductName,
                            Quantity = marketingItem.Quantity,
                            UnitPrice = marketingItem.UnitPrice,
                            CreatedAt = DateTimeOffset.UtcNow
                        };

                        if (marketingItem.Ads != null && marketingItem.Ads.Any())
                        {
                            var adEntities = new List<SimulationAdvertisement>();
                            foreach (var adDto in marketingItem.Ads)
                            {
                                if (string.IsNullOrWhiteSpace(adDto.AdUrl) ||
                                    string.IsNullOrWhiteSpace(adDto.AdType))
                                {
                                    return new GeneralResult
                                    {
                                        IsSuccess = false,
                                        Message = "All product ad data is required",
                                        Data = null
                                    };
                                }

                                var adEntity = new SimulationAdvertisement
                                {
                                    Marketing = marketingEntity,
                                    AdUrl = adDto.AdUrl,
                                    AdType = adDto.AdType,
                                    EndAt = DateTimeOffset.UtcNow.AddHours(24),
                                    CreatedAt = DateTimeOffset.UtcNow
                                };
                                adEntities.Add(adEntity);
                            }


                            marketingEntity.Advertisements = adEntities;
                            _dbContext.SimulationAdvertisements.AddRange(adEntities);
                        }

                        marketingEntities.Add(marketingEntity);
                    }
                    await _dbContext.SimulationMarketings.AddRangeAsync(marketingEntities);
                }

                /***************************** Adding Campaigns *****************************/
                List<SimulationCampaign>? marketingCampaignEntities = null;
                if (dto.MarketingCampaignDto != null && dto.MarketingCampaignDto.Any())
                {
                    foreach (var item in dto.MarketingCampaignDto)
                    {
                        if (string.IsNullOrWhiteSpace(item.Name) ||
                            item.Cost == 0)
                        {
                            return new GeneralResult
                            {
                                IsSuccess = false,
                                Message = "All marketing campaign data is required",
                                Data = null
                            };
                        }
                    }

                    double? costs = 0;
                    foreach (var item in dto.MarketingCampaignDto)
                    {
                        costs += item.Cost;
                    }

                    if (costs > dto.FeasibilityStudyDto.MarketingCost)
                    {
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = "The total marketing campaign cost must be less than or " +
                            "equal to the cost of the marketing identified in the feasibility study.",
                            Data = null
                        };
                    }

                    simulation.TotalCampaignValue = costs;

                    marketingCampaignEntities = dto.MarketingCampaignDto
                        .Where(item => !string.IsNullOrWhiteSpace(item.Name) && item.Cost.HasValue)
                        .Select(item => new SimulationCampaign
                        {
                            Simulation = simulation,
                            Name = item.Name,
                            Cost = item.Cost,
                            CreatedAt = DateTimeOffset.UtcNow
                        }).ToList();

                    await _dbContext.SimulationCampaigns.AddRangeAsync(marketingCampaignEntities);
                }
                /***************************** additional process *****************************/
                simulation.ProjectStatus = SimulationStatus.Pending;
                simulation.IdeaPowerhValue = positiveFactors - negativeFactors;
                await _dbContext.Simulations.AddAsync(simulation);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Simulation created successfully",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while creating a new entity.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to create simulation",
                    Data = null
                };
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> GetAllSimulationsAsync()
        {
            try
            {
                var simulations = await _dbContext.Simulations
                    .Where(s => !s.IsDeleted)
                    .AsNoTracking()
                    .Select(s => new SimulationDetails
                    {
                        Id = s.Id,
                        ProjectField = s.ProjectField,
                        ProjectType = s.ProjectType,
                        ProjectStatus = s.ProjectStatus,
                        IdeaPowerhValue = s.IdeaPowerhValue ?? 0,
                        TotalCampaignValue = s.TotalCampaignValue ?? 0,

                        userSimulationData = new UserSimulationData
                        {
                            FirstName = s.User.FirstName,
                            LastName = s.User.LastName,
                            Email = s.User.Email,
                        },

                        IdeaPowerDto = s.SimulationIdeaPowers!.Select(ip => new IdeaPowerCreateDto
                        {
                            CategoryType = ip.CategoryType ?? 0,
                            CategoryName = ip.CategoryName,
                            FactorData = new List<FactorData>
                            {
                        new FactorData
                        {
                            StrengthFactor = ip.StrengthFactor,
                            FactorScore = ip.FactorScore ?? 0
                        }
                            }
                        }).ToList(),

                        BusinessPlanDto = s.SimulationBusinessPlans!
                            .Select(bp => new BusinessPlanCreateDto
                            {
                                BusinessPlanFileUrl = bp.BusinessPlanFileUrl,
                                BusinessPartners = bp.BusinessPartners,
                                ProjectActivities = bp.ProjectActivities,
                                ValueProposition = bp.ValueProposition,
                                CustomerRelationships = bp.CustomerRelationships,
                                CustomerSegments = bp.CustomerSegments,
                                RequiredResources = bp.RequiredResources,
                                DistributionChannels = bp.DistributionChannels,
                                RevenueStreams = bp.RevenueStreams,
                                CostStructure = bp.CostStructure,
                                CreatedAt = bp.CreatedAt
                            })
                            .FirstOrDefault(),

                        FeasibilityStudyDto = s.SimulationFeasibilityStudies!
                            .Select(fs => new FeasibilityStudyCreateDto
                            {
                                ProjectName = fs.ProjectName,
                                CapitalMin = fs.MaxCapital,
                                CapitalMax = fs.MinCapital,
                                IsInterest = fs.IsInterest,
                                InterestRate = fs.InterestRate,
                                MarketingCost = fs.MarketingCost,
                                RentCost = fs.RentCost,
                                DecorationCost = fs.DecorationCost,
                                EquipmentCost = fs.EquipmentCost,
                                GovFees = fs.GovFees,
                                InventoryCost = fs.InventoryCost,
                                CreatedAt = fs.CreatedAt
                            })
                            .FirstOrDefault(),

                        PurchaseDto = new PurchaseCreateDto
                        {
                            Products = s.SimulationPurchases!.Select(p => new Product
                            {
                                ItemName = p.ItemName,
                                ItemCost = p.ItemCost ?? 0
                            }).ToList(),
                            Description = s.SimulationPurchases!.FirstOrDefault()!.Description
                        },

                        MarketingDto = s.SimulationMarketings!
                            .Select(m => new MarketingCreateDto
                            {
                                ProductName = m.ProductName,
                                Quantity = m.Quantity,
                                UnitPrice = m.UnitPrice,
                                CreatedAt = m.CreatedAt
                            })
                            .ToList(),

                        AdvertisementDto = s.SimulationAdvertisements!
                            .Select(ad => new AdvertisementCreateDto
                            {
                                AdUrl = ad.AdUrl,
                                AdType = ad.AdType,
                                CreatedAt = ad.CreatedAt
                            })
                            .ToList(),

                        MarketingCampaignDto = s.SimulationCampaigns!
                            .Select(c => new CampaignCreateDto
                            {
                                Name = c.Name,
                                Cost = c.Cost,
                                EndAt = c.EndAt,
                                CreatedAt = c.CreatedAt
                            })
                            .ToList()
                    }).ToListAsync();

                if (!simulations.Any())
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No data found",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Simulations retrieved successfully",
                    Data = simulations
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all simulations.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to retrieve simulations",
                    Data = null
                };
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> GetSimulationByIdAsync(int simulationId)
        {
            try
            {
                var simulation = await _dbContext.Simulations
                    .Where(s => s.Id == simulationId && !s.IsDeleted)
                    .AsNoTracking()
                    .Select(s => new SimulationDetails
                    {
                        Id = s.Id,
                        ProjectField = s.ProjectField,
                        ProjectType = s.ProjectType,
                        ProjectStatus = s.ProjectStatus,
                        IdeaPowerhValue = s.IdeaPowerhValue ?? 0,
                        TotalCampaignValue = s.TotalCampaignValue ?? 0,

                        userSimulationData = new UserSimulationData
                        {
                            FirstName = s.User.FirstName,
                            LastName = s.User.LastName,
                            Email = s.User.Email,
                        },

                        IdeaPowerDto = s.SimulationIdeaPowers!.Select(ip => new IdeaPowerCreateDto
                        {
                            CategoryType = ip.CategoryType ?? 0,
                            CategoryName = ip.CategoryName,
                            FactorData = new List<FactorData>
                            {
                        new FactorData
                        {
                            StrengthFactor = ip.StrengthFactor,
                            FactorScore = ip.FactorScore ?? 0
                        }
                            }
                        }).ToList(),

                        BusinessPlanDto = s.SimulationBusinessPlans!
                            .Select(bp => new BusinessPlanCreateDto
                            {
                                BusinessPlanFileUrl = bp.BusinessPlanFileUrl,
                                BusinessPartners = bp.BusinessPartners,
                                ProjectActivities = bp.ProjectActivities,
                                ValueProposition = bp.ValueProposition,
                                CustomerRelationships = bp.CustomerRelationships,
                                CustomerSegments = bp.CustomerSegments,
                                RequiredResources = bp.RequiredResources,
                                DistributionChannels = bp.DistributionChannels,
                                RevenueStreams = bp.RevenueStreams,
                                CostStructure = bp.CostStructure,
                                CreatedAt = bp.CreatedAt
                            })
                            .FirstOrDefault(),

                        FeasibilityStudyDto = s.SimulationFeasibilityStudies!
                            .Select(fs => new FeasibilityStudyCreateDto
                            {
                                ProjectName = fs.ProjectName,
                                CapitalMin = fs.MaxCapital,
                                CapitalMax = fs.MinCapital,
                                IsInterest = fs.IsInterest,
                                InterestRate = fs.InterestRate,
                                MarketingCost = fs.MarketingCost,
                                RentCost = fs.RentCost,
                                DecorationCost = fs.DecorationCost,
                                EquipmentCost = fs.EquipmentCost,
                                GovFees = fs.GovFees,
                                InventoryCost = fs.InventoryCost,
                                CreatedAt = fs.CreatedAt
                            })
                            .FirstOrDefault(),

                        PurchaseDto = new PurchaseCreateDto
                        {
                            Products = s.SimulationPurchases!.Select(p => new Product
                            {
                                ItemName = p.ItemName,
                                ItemCost = p.ItemCost ?? 0
                            }).ToList(),
                            Description = s.SimulationPurchases!.FirstOrDefault()!.Description
                        },

                        MarketingDto = s.SimulationMarketings!
                            .Select(m => new MarketingCreateDto
                            {
                                ProductName = m.ProductName,
                                Quantity = m.Quantity,
                                UnitPrice = m.UnitPrice,
                                CreatedAt = m.CreatedAt
                            })
                            .ToList(),

                        AdvertisementDto = s.SimulationAdvertisements!
                            .Select(ad => new AdvertisementCreateDto
                            {
                                AdUrl = ad.AdUrl,
                                AdType = ad.AdType,
                                CreatedAt = ad.CreatedAt
                            })
                            .ToList(),

                        MarketingCampaignDto = s.SimulationCampaigns!
                            .Select(c => new CampaignCreateDto
                            {
                                Name = c.Name,
                                Cost = c.Cost,
                                EndAt = c.EndAt,
                                CreatedAt = c.CreatedAt
                            })
                            .ToList()
                    }).FirstOrDefaultAsync();

                if (simulation == null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No simulation found",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Simulation data retrieved successfully",
                    Data = simulation
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all simulations.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to retrieve simulation data",
                    Data = null
                };
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> GetSimulationAdsAsync(int simulationId, string userId)
        {
            try
            {
                var user = await _dbContext.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted);
                if (user == false)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User not found or deleted.",
                        Data = null
                    };
                }

                var simulationExists = await _dbContext.Simulations
                    .AnyAsync(s => s.Id == simulationId && !s.IsDeleted && s.UserId == userId);
                if (!simulationExists)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = $"Simulation not found or deleted.",
                        Data = null
                    };
                }

                var ads = await _dbContext.SimulationAdvertisements
                    .Where(ad => !ad.IsDeleted && ad.Marketing.SimulationId == simulationId && ad.Marketing.Simulation.UserId == userId)
                    .Select(ad => new
                    {
                        ad.Id,
                        ad.AdUrl,
                        ad.AdType,
                        ad.EndAt,
                        ad.CreatedAt,
                        Marketing = ad.Marketing == null ? null : new
                        {
                            ad.Marketing.ProductName,
                            ad.Marketing.Quantity,
                            ad.Marketing.UnitPrice
                        }
                    }).ToListAsync();

                if (!ads.Any())
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No ads found for this simulation.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Ads retrieved successfully.",
                    Data = ads
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching ads for simulation {simulationId}.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to retrieve ads.",
                    Data = null
                };
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> GetSimulationsByStatusAsync(SimulationStatus status)
        {
            try
            {
                var simulations = await _dbContext.Simulations
                    .Where(s => s.ProjectStatus == status && !s.IsDeleted)
                    .AsNoTracking()
                    .Select(s => new SimulationDetails
                    {
                        Id = s.Id,
                        ProjectField = s.ProjectField,
                        ProjectType = s.ProjectType,
                        ProjectStatus = s.ProjectStatus,
                        IdeaPowerhValue = s.IdeaPowerhValue ?? 0,
                        TotalCampaignValue = s.TotalCampaignValue ?? 0,

                        userSimulationData = new UserSimulationData
                        {
                            FirstName = s.User.FirstName,
                            LastName = s.User.LastName,
                            Email = s.User.Email,
                        },

                        IdeaPowerDto = s.SimulationIdeaPowers!.Select(ip => new IdeaPowerCreateDto
                        {
                            CategoryType = ip.CategoryType ?? 0,
                            CategoryName = ip.CategoryName,
                            FactorData = new List<FactorData>
                            {
                        new FactorData
                        {
                            StrengthFactor = ip.StrengthFactor,
                            FactorScore = ip.FactorScore ?? 0
                        }
                            }
                        }).ToList(),

                        BusinessPlanDto = s.SimulationBusinessPlans!
                            .Select(bp => new BusinessPlanCreateDto
                            {
                                BusinessPlanFileUrl = bp.BusinessPlanFileUrl,
                                BusinessPartners = bp.BusinessPartners,
                                ProjectActivities = bp.ProjectActivities,
                                ValueProposition = bp.ValueProposition,
                                CustomerRelationships = bp.CustomerRelationships,
                                CustomerSegments = bp.CustomerSegments,
                                RequiredResources = bp.RequiredResources,
                                DistributionChannels = bp.DistributionChannels,
                                RevenueStreams = bp.RevenueStreams,
                                CostStructure = bp.CostStructure,
                                CreatedAt = bp.CreatedAt
                            })
                            .FirstOrDefault(),

                        FeasibilityStudyDto = s.SimulationFeasibilityStudies!
                            .Select(fs => new FeasibilityStudyCreateDto
                            {
                                ProjectName = fs.ProjectName,
                                CapitalMin = fs.MaxCapital,
                                CapitalMax = fs.MinCapital,
                                IsInterest = fs.IsInterest,
                                InterestRate = fs.InterestRate,
                                MarketingCost = fs.MarketingCost,
                                RentCost = fs.RentCost,
                                DecorationCost = fs.DecorationCost,
                                EquipmentCost = fs.EquipmentCost,
                                GovFees = fs.GovFees,
                                InventoryCost = fs.InventoryCost,
                                CreatedAt = fs.CreatedAt
                            })
                            .FirstOrDefault(),

                        PurchaseDto = new PurchaseCreateDto
                        {
                            Products = s.SimulationPurchases!.Select(p => new Product
                            {
                                ItemName = p.ItemName,
                                ItemCost = p.ItemCost ?? 0
                            }).ToList(),
                            Description = s.SimulationPurchases!.FirstOrDefault()!.Description
                        },

                        MarketingDto = s.SimulationMarketings!
                            .Select(m => new MarketingCreateDto
                            {
                                ProductName = m.ProductName,
                                Quantity = m.Quantity,
                                UnitPrice = m.UnitPrice,
                                CreatedAt = m.CreatedAt
                            })
                            .ToList(),

                        AdvertisementDto = s.SimulationAdvertisements!
                            .Select(ad => new AdvertisementCreateDto
                            {
                                AdUrl = ad.AdUrl,
                                AdType = ad.AdType,
                                CreatedAt = ad.CreatedAt
                            })
                            .ToList(),

                        MarketingCampaignDto = s.SimulationCampaigns!
                            .Select(c => new CampaignCreateDto
                            {
                                Name = c.Name,
                                Cost = c.Cost,
                                EndAt = c.EndAt,
                                CreatedAt = c.CreatedAt
                            })
                            .ToList()
                    }).ToListAsync();

                if (!simulations.Any())
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No simulations found",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Simulations data retrieved successfully",
                    Data = simulations
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving all simulations with status {status}.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to retrieve simulations data",
                    Data = null
                };
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> EntreLaunchdateSimulationStatusAsync(int simulationId, SimulationStatus newStatus)
        {
            try
            {
                if (string.IsNullOrEmpty(Enum.GetName(typeof(SimulationStatus), newStatus)))
                {
                    return new GeneralResult(false, "Status cannot be empty", null);
                }

                if (Enum.IsDefined(typeof(SimulationStatus), newStatus) == false)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Invalid status. Allowed values: Pending, Approved, Rejected",
                        Data = null
                    };
                }

                var simulation = await _dbContext.Simulations
                    .FirstOrDefaultAsync(s => s.Id == simulationId && !s.IsDeleted);

                if (simulation == null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Simulation not found",
                        Data = null
                    };
                }

                if (simulation.ProjectStatus == newStatus)
                {
                    return new GeneralResult
                    {
                        IsSuccess = true,
                        Message = "Simulation status is already EntreLaunchdated",
                        Data = null
                    };
                }

                simulation.ProjectStatus = newStatus;
                simulation.EntreLaunchdatedAt = DateTimeOffset.UtcNow;

                _dbContext.Simulations.EntreLaunchdate(simulation);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = $"Simulation status EntreLaunchdated to {newStatus} successfully",
                    Data = new { simulationId, NewStatus = newStatus }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while EntreLaunchdating simulation status ID {simulationId}");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to EntreLaunchdate simulation status",
                    Data = null
                };
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> RegisterGuestAsync(GuestRegisterDto dto)
        {
            try
            {
                var existingUser = await _dbContext.Guests
                    .AnyAsync(u => u.PhoneNumber == dto.PhoneNumber);
                if (existingUser == true)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Guest user already exists.",
                        Data = null
                    };
                }

                var user = new Guest
                {
                    Name = dto.Name,
                    PhoneNumber = dto.PhoneNumber,
                    Email = dto.Email
                };

                var createResult = await _dbContext.Guests.AddAsync(user);
                if (createResult == null)
                {
                    _logger.LogError("Error creating gest user");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Error adding gest user data",
                        Data = null
                    };
                }

                await _dbContext.SaveChangesAsync();
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Registration successful",
                    Data = new GuestDetails
                    {
                        Id = user.Id.ToString(),
                        Name = user.Name,
                        PhoneNumber = user.PhoneNumber
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering guest");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Error registering guest",
                    Data = null
                };
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> LikeProductAdAsync(int adId, string userId)
        {
            try
            {
                var existingUser = await _dbContext.Guests
                    .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
                if (existingUser == null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Guest user not found",
                        Data = null
                    };
                }

                var ad = await _dbContext.SimulationAdvertisements
                    .Where(ad => ad.Id == adId && !ad.IsDeleted).FirstOrDefaultAsync();
                if (ad == null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Product ad not found",
                        Data = null
                    };
                }

                var existingLike = await _dbContext.SimulationAdLikes
                    .FirstOrDefaultAsync(l => l.AdId == adId && l.GuestId == userId.ToString() &&
                    !l.IsDeleted);

                if (existingLike == null)
                {
                    // get all ad likes
                    var activeLikesCount = await _dbContext.SimulationAdLikes
                        .CountAsync(l => l.AdId == adId && l.IsActive && !l.IsDeleted);

                    if (activeLikesCount >= ad.Marketing.Quantity)
                    {
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = "No more units available for this product.",
                            Data = null
                        };
                    }

                    // add new like
                    var newLike = new SimulationAdLike
                    {
                        AdId = adId,
                        GuestId = userId.ToString(),
                        IsActive = true,
                        CreatedAt = DateTimeOffset.UtcNow
                    };
                    _dbContext.SimulationAdLikes.Add(newLike);
                    await _dbContext.SaveChangesAsync();
                    return new GeneralResult
                    {
                        IsSuccess = true,
                        Message = "Product ad liked successfully",
                        Data = null
                    };
                }
                else
                {
                    // active like
                    if (!existingLike.IsActive)
                    {
                        var activeLikesCount = await _dbContext.SimulationAdLikes
                            .CountAsync(l => l.AdId == adId && l.IsActive && !l.IsDeleted);

                        if (activeLikesCount >= ad.Marketing.Quantity)
                        {
                            return new GeneralResult
                            {
                                IsSuccess = false,
                                Message = "No more units available for this product.",
                                Data = null
                            };
                        }

                        existingLike.IsActive = true;
                        existingLike.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                        await _dbContext.SaveChangesAsync();
                        return new GeneralResult
                        {
                            IsSuccess = true,
                            Message = "Product ad liked successfully",
                            Data = null
                        };
                    }
                    else
                    {
                        //deactivate like
                        existingLike.IsActive = false;
                        existingLike.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                        await _dbContext.SaveChangesAsync();
                        return new GeneralResult
                        {
                            IsSuccess = true,
                            Message = "Product ad UnLiked successfully",
                            Data = null
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error liking product ad");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to like product ad",
                    Data = null
                };
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> GetAdLikeCountAsync(int adId)
        {
            try
            {
                var count = await _dbContext.SimulationAdLikes
                    .CountAsync(l => l.AdId == adId && l.IsActive);

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Success",
                    Data = count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ad like count");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get ad like count",
                    Data = null
                };
            }
        }

        /*
        public async Task<GeneralResult> GenerateFinalReportAsync(int simulationId, string userId)
        {
            try
            {
                var user = await _dbContext.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted);
                if (user == false)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User not found or deleted.",
                        Data = null
                    };
                }

                var simulation = await _dbContext.Simulations
                    .Include(s => s.SimulationFeasibilityStudies)
                    .Include(s => s.SimulationPurchases)
                    .Include(s => s.SimulationMarketings)
                    .Include(s => s.SimulationCampaigns)
                    .Include(s => s.SimulationDeviations)
                    .FirstOrDefaultAsync(s => s.Id == simulationId && !s.IsDeleted && s.UserId == userId);

                if (simulation == null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Simulation not found or deleted.",
                        Data = null
                    };
                }

                if (!string.Equals(simulation.UserId, userId, StringComparison.OrdinalIgnoreCase))
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Simulation not found or deleted.",
                        Data = null
                    };
                }

                var feasibility = simulation.SimulationFeasibilityStudies?.FirstOrDefault();
                var purchases = simulation.SimulationPurchases ?? new List<SimulationPurchase>();
                var marketings = simulation.SimulationMarketings ?? new List<SimulationMarketing>();
                var campaigns = simulation.SimulationCampaigns ?? new List<SimulationCampaign>();
                var deviations = simulation.SimulationDeviations ?? new List<SimulationDeviation>();

                double totalPurchasesCost = purchases.Sum(p => p.ItemCost ?? 0);
                double totalCampaignCost = campaigns.Sum(c => c.Cost ?? 0);
                double totalDeviationPos = deviations.Where(d => d.Type == "Positive").Sum(d => d.Amount ?? 0);
                double totalDeviationNeg = deviations.Where(d => d.Type == "Negative").Sum(d => d.Amount ?? 0);
                double marketingCost = feasibility?.MarketingCost ?? 0;

                // 8) حساب الأرباح والمبيعات (مثال)
                double netSales = marketings.Sum(m => (m.Quantity ?? 0) * (m.UnitPrice ?? 0));
                // الربح النهائي (FinalProfit) =  netSales - (purchases + campaigns + ... ) ± الانحراف!
                double finalProfit = netSales
                                  - totalPurchasesCost
                                  - totalCampaignCost
                                  + totalDeviationPos
                                  - totalDeviationNeg;

                // 9) نسبة الأرباح (مثال)
                double capitalMax = feasibility?.MaxCapital ?? 1;
                double profitPercentage = (finalProfit / capitalMax) * 100;
                // (أو أي معادلة أخرى)

                // 10) تقدير مستوى المخاطرة (risk)
                string riskLevel;
                if (profitPercentage >= 20)
                    riskLevel = "منخفض";
                else if (profitPercentage >= 10)
                    riskLevel = "متوسط";
                else
                    riskLevel = "عالٍ";

                // 11) تعبئة الـ DTO النهائي
                var reportDto = new FinalReportDto
                {
                    ProjectName = feasibility?.ProjectName ?? "N/A",
                    CapitalMin = feasibility?.MinCapital,
                    CapitalMax = feasibility?.MaxCapital,
                    MarketingCost = feasibility?.MarketingCost,
                    PurchasesCost = totalPurchasesCost,
                    CampaignCost = totalCampaignCost,
                    DeviationPositive = totalDeviationPos,
                    DeviationNegative = totalDeviationNeg,
                    FinalProfit = finalProfit,
                    ProfitPercentage = profitPercentage,
                    RiskLevel = riskLevel,
                    InventoryCost = feasibility?.InventoryCost,
                    IsInterest = feasibility?.IsInterest ?? false,
                    InterestRate = feasibility?.InterestRate,
                    NetSales = netSales,
                };

                // ممكن حساب remaining budget الخ..
                reportDto.RemainingBudget = (feasibility?.MaxCapital ?? 0)
                                           - totalPurchasesCost
                                           - (feasibility?.MarketingCost ?? 0)
                                           - totalCampaignCost
                                           + (totalDeviationPos - totalDeviationNeg);

                // 12) إعادة النتيجة
                return new GeneralResult(true, "Final report generated successfully", reportDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating final report for simulation {simulationId}");
                return new GeneralResult(false, "Failed to generate final report", ex.Message);
            }
        }
        */
    }
}
