global using System;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.Diagnostics;
global using System.IO;
global using System.Linq;
global using System.Reflection;
global using System.Threading.Tasks;
global using System.Security.Claims;
global using System.Text.Encodings.Web;

// microsoft packages
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.AspNetCore.Diagnostics;
global using Microsoft.Extensions.Options;

global using Npgsql;
global using OfficeOpenXml;
global using Quartz;
global using Serilog.Exceptions;
global using Serilog.Sinks.Elasticsearch;
global using Serilog;
global using AutoMapper;
global using FluentValidation;
global using FluentValidation.AspNetCore;

// other packages
global using EntreLaunch.Data;
global using EntreLaunch.DataAnnotations;
global using EntreLaunch.DTOs;
global using EntreLaunch.Entities;
global using EntreLaunch.Enums;
global using EntreLaunch.Exceptions;
global using EntreLaunch.Helpers;
global using EntreLaunch.Infrastructure;
global using EntreLaunch.Configuration;
global using EntreLaunch.Geography;
global using EntreLaunch.Controllers.BaseAPI;
global using EntreLaunch.Web.Controllers.AuthenticationAPI;
global using EntreLaunch.Extensions;
global using EntreLaunch.Filters;
global using EntreLaunch.Formatters.Csv;
global using EntreLaunch.Middlewares;
global using EntreLaunch.Tasks;

// services
global using EntreLaunch.Services;
global using EntreLaunch.Services.MyPartnerSvc;
global using EntreLaunch.Services.MyOpportunitySvc;
global using EntreLaunch.Services.EmailSvc;
global using EntreLaunch.Services.TrainingSvc;
global using EntreLaunch.Services.AuthenticationSvc;
global using EntreLaunch.Services.AothrizationSvc;
global using EntreLaunch.Services.BaseSvc;
global using EntreLaunch.Services.MyTeamSvc;
global using EntreLaunch.Services.MyFinancingSvc;
global using EntreLaunch.Services.PaymentSvc;
global using EntreLaunch.Services.ImportSvc;
global using EntreLaunch.Services.MyCommunitySvc;
global using EntreLaunch.Services.CacheSvc;
global using EntreLaunch.Services.ClubSvc;
global using EntreLaunch.Services.ConsultationSvc;
global using EntreLaunch.Services.ContactSvc;
global using EntreLaunch.Services.ExportSvc;
global using EntreLaunch.Services.TaskSvc;

// interfaces
global using EntreLaunch.Interfaces.MyOpportunityIntf;
global using EntreLaunch.Interfaces.MyPartnerIntf;
global using EntreLaunch.Interfaces;
global using EntreLaunch.Interfaces.AuthrizationIntf;
global using EntreLaunch.Interfaces.AuthenticationIntf;
global using EntreLaunch.Interfaces.ConsultationIntf;
global using EntreLaunch.Interfaces.EmailIntf;
global using EntreLaunch.Interfaces.CacheIntf;
global using EntreLaunch.Interfaces.ImportIntf;
global using EntreLaunch.Interfaces.PaymentIntf;
global using EntreLaunch.Interfaces.TrainingIntf;
global using EntreLaunch.Interfaces.ClubIntf;
global using EntreLaunch.Interfaces.MyCommunityIntf;
global using EntreLaunch.Interfaces.MyFinancingIntf;
global using EntreLaunch.Interfaces.MyTeamIntf;
global using EntreLaunch.Interfaces.SimulationIntf;
global using EntreLaunch.Interfaces.TaskIntf;
global using EntreLaunch.Interfaces.UserIntf;

// constants
global using static EntreLaunch.Infrastructure.Permissions;
global using ModelSnapshot = Microsoft.EntityFrameworkCore.Infrastructure.ModelSnapshot;
