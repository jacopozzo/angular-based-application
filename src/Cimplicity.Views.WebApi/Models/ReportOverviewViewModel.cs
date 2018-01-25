using System.Data;
using AutoMapper;
using Cimplicity.Views.Domain.Model;
using Utils.Extensions.Data;

namespace Cimplicity.Views.WebApi.Models
{
    public class ReportOverviewMapProfile : Profile
    {
        public ReportOverviewMapProfile()
        {
            CreateMap<ReportOverview, ReportOverviewViewModel>()
                .ForMember(target => target.Actual, opt => opt.MapFrom(source => source.Actual))
                .ForMember(target => target.OverflowRemaining, opt => opt.MapFrom(source => source.Overflow.Remaining))
                .ForMember(target => target.OverflowSet, opt => opt.MapFrom(source => source.Overflow.Set))
                .ForMember(target => target.Remaining, opt => opt.MapFrom(source => source.Remaining))
                .ForMember(target => target.RuleName, opt => opt.MapFrom(source => source.Rule.Name))
                .ForMember(target => target.Set, opt => opt.MapFrom(source => source.Set))
                .ForMember(target => target.WorkCell, opt => opt.MapFrom(source => source.WorkCell))
                .ForMember(target => target.WorkUnit, opt => opt.MapFrom(source => source.WorkUnit));

            CreateMap<DataRow, ReportOverviewViewModel>()
                .ForMember(target => target.Actual,
                    opt => opt.MapFrom(source =>
                        source.GetValueByColumnName<int?>(nameof(ReportOverviewViewModel.Actual), false)))
                .ForMember(target => target.OverflowRemaining,
                    opt => opt.MapFrom(source =>
                        source.GetValueByColumnName<int?>(nameof(ReportOverviewViewModel.OverflowRemaining), false)))
                .ForMember(target => target.OverflowSet,
                    opt => opt.MapFrom(source =>
                        source.GetValueByColumnName<int?>(nameof(ReportOverviewViewModel.OverflowSet), false)))
                .ForMember(target => target.Remaining,
                    opt => opt.MapFrom(source =>
                        source.GetValueByColumnName<int?>(nameof(ReportOverviewViewModel.Remaining), false)))
                .ForMember(target => target.RuleName,
                    opt => opt.MapFrom(source =>
                        source.GetValueByColumnName<string>(nameof(ReportOverviewViewModel.RuleName), false)))
                .ForMember(target => target.Set,
                    opt => opt.MapFrom(source =>
                        source.GetValueByColumnName<int?>(nameof(ReportOverviewViewModel.Set), false)))
                .ForMember(target => target.WorkCell,
                    opt => opt.MapFrom(source =>
                        source.GetValueByColumnName<string>(nameof(ReportOverviewViewModel.WorkCell), false)))
                .ForMember(target => target.WorkUnit,
                    opt => opt.MapFrom(source =>
                        source.GetValueByColumnName<string>(nameof(ReportOverviewViewModel.WorkUnit), false)));

        }
    }

    public class ReportOverviewViewModel
    {
        public string WorkCell { get; set; }
        public string WorkUnit { get; set; }
        public int? Actual { get; set; }
        public int? Remaining { get; set; }
        public int? Set { get; set; }
        public int? OverflowRemaining { get; set; }
        public int? OverflowSet { get; set; }
        public string RuleName { get; set; }
    }
}