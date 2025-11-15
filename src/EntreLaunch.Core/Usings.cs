// system packages
global using System.ComponentModel;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
global using System.Diagnostics;
global using System.Reflection;
global using System.Text;
global using System.Text.Encodings.Web;
global using System.Text.Json;
global using System.Net.Http.Headers;
global using System.Net.Mail;
global using System.Net;
global using System.Globalization;
global using System.Linq.Expressions;
global using System.Security.Cryptography;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using System.Text.Json.Serialization;
global using System.Collections.Concurrent;


// EntreLaunch
global using EntreLaunch.Data;
global using EntreLaunch.DataAnnotations;
global using EntreLaunch.DTOs;
global using EntreLaunch.Entities;
global using EntreLaunch.Enums;
global using EntreLaunch.Exceptions;
global using EntreLaunch.Helpers;
global using EntreLaunch.Infrastructure;
global using EntreLaunch.Geography;
global using EntreLaunch.Configuration;

// microsoft packages
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Logging;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.AspNetCore.DataProtection;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.EntityFrameworkCore.DynamicLinq;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using EntreLaunch.Interfaces.UserIntf;

// others packages
global using FluentValidation;
global using OfficeOpenXml;
global using DnsClient;
global using DnsClient.Protocol;
global using HtmlAgilityPack;
global using MailKit;
global using MailKit.Security;
global using MimeKit;
global using EntreLaunch.Validations;
global using AutoMapper;
global using CsvHelper;
global using CsvHelper.Configuration;
global using Nest;
global using Serilog;
global using Medallion.Threading.Postgres;

// services
global using EntreLaunch.Services.ImportSvc;
global using EntreLaunch.Services.MyOpportunitySvc;
global using EntreLaunch.Services.BaseSvc;
global using EntreLaunch.Services;

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
global using EntreLaunch.Interfaces.SimulationIntf;
