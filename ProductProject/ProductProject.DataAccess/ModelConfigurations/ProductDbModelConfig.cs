using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductProject.DataAccess.Common.Models;

namespace ProductProject.DataAccess.ModelConfigurations
{
    public class ProductDbModelConfig : EntityTypeConfiguration<DbProduct>
    {
        public ProductDbModelConfig()
        {
            ToTable("Production.Product");
            HasKey(k => k.ProductID);
            Property(p => p.Name).IsRequired().IsUnicode().IsVariableLength();
            Property(p => p.ProductNumber).IsRequired().IsUnicode().IsVariableLength();
            Property(p => p.MakeFlag).IsRequired();
            Property(p => p.FinishedGoodsFlag).IsRequired();
            Property(p => p.Color).IsRequired().IsUnicode().IsVariableLength();
            Property(p => p.SafetyStockLevel).IsRequired();
            Property(p => p.ReorderPoint).IsRequired();
            Property(p => p.StandardCost).IsRequired();
            Property(p => p.ListPrice).IsRequired();
            Property(p => p.Size).IsRequired().IsUnicode().IsVariableLength();
            Property(p => p.SizeUnitMeasureCode).IsRequired().IsUnicode().IsVariableLength();
            Property(p => p.WeightUnitMeasureCode).IsRequired().IsUnicode().IsVariableLength();
            Property(p => p.Weight).IsOptional();
            Property(p => p.DaysToManufacture).IsRequired();
            Property(p => p.ProductLine).IsRequired().IsUnicode().IsVariableLength();
            Property(p => p.Class).IsRequired().IsUnicode().IsVariableLength();
            Property(p => p.Style).IsRequired().IsUnicode().IsVariableLength();
            Property(p => p.ProductSubcategoryID).IsOptional();
            Property(p => p.ProductModelID).IsOptional();
            Property(p => p.SellStartDate).IsRequired();
            Property(p => p.SellEndDate).IsOptional();
            Property(p => p.DiscontinuedDate).IsOptional();
            Property(p => p.rowguid).IsRequired();
            Property(p => p.ModifiedDate).IsRequired();
        }
    }
}
