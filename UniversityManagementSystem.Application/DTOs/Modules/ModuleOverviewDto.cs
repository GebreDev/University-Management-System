using System.Collections.Generic;

namespace UniversityManagementSystem.Application.DTOs.Modules
{
    /// <summary>One stat card shown on a module overview page.</summary>
    public class ModuleStatDto
    {
        public string Label { get; set; } = "";
        public string Value { get; set; } = "";
        public string Sub { get; set; } = "";
        public string Icon { get; set; } = "fas fa-circle";
        public string Gradient { get; set; } = "stat-grad-a";
    }

    /// <summary>A quick action linking to a real page of the app.</summary>
    public class ModuleLinkDto
    {
        public string Label { get; set; } = "";
        public string Icon { get; set; } = "fas fa-arrow-right";
        public string Url { get; set; } = "#";
        public string Description { get; set; } = "";
    }

    /// <summary>A real-data row (event, notification, schedule, course...).</summary>
    public class ModuleItemDto
    {
        public string Title { get; set; } = "";
        public string Meta { get; set; } = "";
        public string Date { get; set; } = "";
        public string Icon { get; set; } = "fas fa-star";
        public string Color { get; set; } = "primary";
        public string Url { get; set; } = "";
    }

    /// <summary>Full payload rendered by the generic module overview page.</summary>
    public class ModuleOverviewDto
    {
        public string Key { get; set; } = "";
        public string Title { get; set; } = "";
        public string Subtitle { get; set; } = "";
        public string Icon { get; set; } = "fas fa-cube";
        public string Gradient { get; set; } = "stat-grad-a";
        public string Description { get; set; } = "";
        public List<ModuleStatDto> Stats { get; set; } = new();
        public List<ModuleLinkDto> QuickLinks { get; set; } = new();
        public List<ModuleItemDto> Items { get; set; } = new();
        public string ItemsTitle { get; set; } = "";
    }
}
