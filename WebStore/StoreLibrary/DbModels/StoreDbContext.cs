using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StoreLibrary.DbModels;

public partial class StoreDbContext : DbContext
{
    public StoreDbContext()
    {
    }

    public StoreDbContext(DbContextOptions<StoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Campaign> Campaigns { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartHistory> CartHistories { get; set; }

    public virtual DbSet<CartProdut> CartProduts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Favourite> Favourites { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Purchase> Purchases { get; set; }

    public virtual DbSet<PurchaseProduct> PurchaseProducts { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<UserImage> UserImages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=StoreDB;TrustServerCertificate=True;MultipleActiveResultSets=true;Trusted_Connection=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.PkAddress);

            entity.ToTable("Address");

            entity.Property(e => e.PkAddress)
                .ValueGeneratedNever()
                .HasColumnName("pk_address");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .HasColumnName("country");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.FkUser).HasColumnName("fk_user");
            entity.Property(e => e.FullAddress)
                .HasMaxLength(50)
                .HasColumnName("full_address");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.HasKey(e => e.PkCampaign);

            entity.ToTable("Campaign");

            entity.Property(e => e.PkCampaign)
                .ValueGeneratedNever()
                .HasColumnName("pk_campaign");
            entity.Property(e => e.DateEnd).HasColumnName("date_end");
            entity.Property(e => e.DateStart).HasColumnName("date_start");

            entity.HasMany(d => d.FkImages).WithMany(p => p.FkCampaigns)
                .UsingEntity<Dictionary<string, object>>(
                    "CampaignImage",
                    r => r.HasOne<Image>().WithMany()
                        .HasForeignKey("FkImage")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CampaignImage_Image"),
                    l => l.HasOne<Campaign>().WithMany()
                        .HasForeignKey("FkCampaign")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CampaignImage_Campaign"),
                    j =>
                    {
                        j.HasKey("FkCampaign", "FkImage");
                        j.ToTable("CampaignImage");
                        j.IndexerProperty<int>("FkCampaign").HasColumnName("fk_campaign");
                        j.IndexerProperty<int>("FkImage").HasColumnName("fk_image");
                    });

            entity.HasMany(d => d.FkProducts).WithMany(p => p.FkCampaigns)
                .UsingEntity<Dictionary<string, object>>(
                    "CampaignProduct",
                    r => r.HasOne<Product>().WithMany()
                        .HasForeignKey("FkProduct")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CampaignProduct_Product"),
                    l => l.HasOne<Campaign>().WithMany()
                        .HasForeignKey("FkCampaign")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CampaignProduct_Campaign"),
                    j =>
                    {
                        j.HasKey("FkCampaign", "FkProduct");
                        j.ToTable("CampaignProduct");
                        j.IndexerProperty<int>("FkCampaign").HasColumnName("fk_campaign");
                        j.IndexerProperty<int>("FkProduct").HasColumnName("fk_product");
                    });
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.PkCart);

            entity.ToTable("Cart");

            entity.Property(e => e.PkCart)
                .ValueGeneratedNever()
                .HasColumnName("pk_cart");
            entity.Property(e => e.FkPurchase).HasColumnName("fk_purchase");
            entity.Property(e => e.FkUser).HasColumnName("fk_user");

            entity.HasOne(d => d.FkPurchaseNavigation).WithMany(p => p.Carts)
                .HasForeignKey(d => d.FkPurchase)
                .HasConstraintName("FK_Cart_Purchase");
        });

        modelBuilder.Entity<CartHistory>(entity =>
        {
            entity.HasKey(e => e.PkCart);

            entity.ToTable("CartHistory");

            entity.Property(e => e.PkCart)
                .ValueGeneratedNever()
                .HasColumnName("pk_cart");
            entity.Property(e => e.FkPurchase).HasColumnName("fk_purchase");
            entity.Property(e => e.FkReview).HasColumnName("fk_review");
            entity.Property(e => e.FkUser).HasColumnName("fk_user");

            entity.HasOne(d => d.FkPurchaseNavigation).WithMany(p => p.CartHistories)
                .HasForeignKey(d => d.FkPurchase)
                .HasConstraintName("FK_CartHistory_Purchase");

            entity.HasOne(d => d.FkReviewNavigation).WithMany(p => p.CartHistories)
                .HasForeignKey(d => d.FkReview)
                .HasConstraintName("FK_CartHistory_Review");
        });

        modelBuilder.Entity<CartProdut>(entity =>
        {
            entity.HasKey(e => new { e.FkCart, e.FkEan });

            entity.ToTable("CartProdut");

            entity.Property(e => e.FkCart).HasColumnName("fk_cart");
            entity.Property(e => e.FkEan).HasColumnName("fk_ean");

            entity.HasOne(d => d.FkCartNavigation).WithMany(p => p.CartProduts)
                .HasForeignKey(d => d.FkCart)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartProdut_Cart");

            entity.HasOne(d => d.FkCart1).WithMany(p => p.CartProduts)
                .HasForeignKey(d => d.FkCart)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartProdut_CartHistory");

            entity.HasOne(d => d.FkEanNavigation).WithMany(p => p.CartProduts)
                .HasForeignKey(d => d.FkEan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartProdut_Product");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.PkCategory);

            entity.ToTable("Category");

            entity.Property(e => e.PkCategory)
                .ValueGeneratedNever()
                .HasColumnName("pk_category");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Favourite>(entity =>
        {
            entity.HasKey(e => new { e.FkProduct, e.FkUser });

            entity.ToTable("Favourite");

            entity.Property(e => e.FkProduct).HasColumnName("fk_product");
            entity.Property(e => e.FkUser).HasColumnName("fk_user");

            entity.HasOne(d => d.FkProductNavigation).WithMany(p => p.Favourites)
                .HasForeignKey(d => d.FkProduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Favourite_Product");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.PkImage);

            entity.ToTable("Image");

            entity.Property(e => e.PkImage)
                .ValueGeneratedNever()
                .HasColumnName("pk_image");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.PathImg)
                .HasMaxLength(50)
                .HasColumnName("path_img");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.PkInvoice);

            entity.ToTable("Invoice");

            entity.Property(e => e.PkInvoice)
                .ValueGeneratedNever()
                .HasColumnName("pk_invoice");
            entity.Property(e => e.DateInvoice).HasColumnName("date_invoice");
            entity.Property(e => e.FkAddressInvoice).HasColumnName("fk_address_invoice");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Nif).HasColumnName("nif");
            entity.Property(e => e.PaypallConfirmation)
                .HasMaxLength(50)
                .HasColumnName("paypall_confirmation");

            entity.HasOne(d => d.FkAddressInvoiceNavigation).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.FkAddressInvoice)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoice_Address");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.PkProduct);

            entity.ToTable("Product");

            entity.Property(e => e.PkProduct)
                .ValueGeneratedNever()
                .HasColumnName("pk_product");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .HasColumnName("description");
            entity.Property(e => e.Ean)
                .HasMaxLength(50)
                .HasColumnName("ean");
            entity.Property(e => e.FkImage).HasColumnName("fk_image");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Stock).HasColumnName("stock");
            entity.Property(e => e.Toggle).HasColumnName("toggle");

            entity.HasOne(d => d.FkImageNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.FkImage)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Image");

            entity.HasMany(d => d.FkCategories).WithMany(p => p.FkProducts)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("FkCategory")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ProductCategory_Category"),
                    l => l.HasOne<Product>().WithMany()
                        .HasForeignKey("FkProduct")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ProductCategory_Product"),
                    j =>
                    {
                        j.HasKey("FkProduct", "FkCategory");
                        j.ToTable("ProductCategory");
                        j.IndexerProperty<int>("FkProduct").HasColumnName("fk_product");
                        j.IndexerProperty<int>("FkCategory").HasColumnName("fk_category");
                    });
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => e.PkPurchase);

            entity.ToTable("Purchase");

            entity.Property(e => e.PkPurchase)
                .ValueGeneratedNever()
                .HasColumnName("pk_purchase");
            entity.Property(e => e.DatePurchase).HasColumnName("date_purchase");
            entity.Property(e => e.FkAddressShipment).HasColumnName("fk_address_shipment");
            entity.Property(e => e.FkInvoice).HasColumnName("fk_invoice");
            entity.Property(e => e.FkReview).HasColumnName("fk_review");
            entity.Property(e => e.FkUser).HasColumnName("fk_user");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.HasOne(d => d.FkAddressShipmentNavigation).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.FkAddressShipment)
                .HasConstraintName("FK_Purchase_Address");

            entity.HasOne(d => d.FkInvoiceNavigation).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.FkInvoice)
                .HasConstraintName("FK_Purchase_Invoice");

            entity.HasOne(d => d.FkReviewNavigation).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.FkReview)
                .HasConstraintName("FK_Purchase_Review");
        });

        modelBuilder.Entity<PurchaseProduct>(entity =>
        {
            entity.HasKey(e => new { e.PkPurchase, e.PkProduct });

            entity.ToTable("PurchaseProduct");

            entity.Property(e => e.PkPurchase).HasColumnName("pk_purchase");
            entity.Property(e => e.PkProduct).HasColumnName("pk_product");
            entity.Property(e => e.FkReview).HasColumnName("fk_review");
            entity.Property(e => e.FkUser).HasColumnName("fk_user");
            entity.Property(e => e.Qtt).HasColumnName("qtt");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.HasOne(d => d.FkReviewNavigation).WithMany(p => p.PurchaseProducts)
                .HasForeignKey(d => d.FkReview)
                .HasConstraintName("FK_PurchaseProduct_Review");

            entity.HasOne(d => d.PkProductNavigation).WithMany(p => p.PurchaseProducts)
                .HasForeignKey(d => d.PkProduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseProduct_Product");

            entity.HasOne(d => d.PkPurchaseNavigation).WithMany(p => p.PurchaseProducts)
                .HasForeignKey(d => d.PkPurchase)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseProduct_Purchase");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.PkReview);

            entity.ToTable("Review");

            entity.Property(e => e.PkReview)
                .ValueGeneratedNever()
                .HasColumnName("pk_review");
            entity.Property(e => e.Comment)
                .HasMaxLength(50)
                .HasColumnName("comment");
            entity.Property(e => e.DataReview).HasColumnName("data_review");
            entity.Property(e => e.Stars).HasColumnName("stars");
            entity.Property(e => e.Toggle).HasColumnName("toggle");

            entity.HasMany(d => d.FkImages).WithMany(p => p.FkReviews)
                .UsingEntity<Dictionary<string, object>>(
                    "ReviewImage",
                    r => r.HasOne<Image>().WithMany()
                        .HasForeignKey("FkImage")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ReviewImages_Image"),
                    l => l.HasOne<Review>().WithMany()
                        .HasForeignKey("FkReview")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ReviewImages_Review"),
                    j =>
                    {
                        j.HasKey("FkReview", "FkImage");
                        j.ToTable("ReviewImages");
                        j.IndexerProperty<int>("FkReview").HasColumnName("fk_review");
                        j.IndexerProperty<int>("FkImage").HasColumnName("fk_image");
                    });
        });

        modelBuilder.Entity<UserImage>(entity =>
        {
            entity.HasKey(e => new { e.FkImage, e.FkUser });

            entity.ToTable("UserImage");

            entity.Property(e => e.FkImage).HasColumnName("fk_image");
            entity.Property(e => e.FkUser).HasColumnName("fk_user");

            entity.HasOne(d => d.FkImageNavigation).WithMany(p => p.UserImages)
                .HasForeignKey(d => d.FkImage)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserImage_Image");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
