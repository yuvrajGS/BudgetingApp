using BudgetingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetingApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("NOW()");
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Groceries", Description = "Supermarkets and food stores", Keywords = "grocery,supermarket,food,produce,organic,market"},
                new Category { Id = 2, Name = "Dining", Description = "Restaurants, cafes, and takeout", Keywords = "restaurant,cafe,coffee,takeout,delivery,fast food,pizza,sushi,bistro"},
                new Category { Id = 3, Name = "Transport", Description = "Gas, transit, rideshare, parking", Keywords = "gas,fuel,uber,lyft,taxi,transit,subway,bus,parking,toll,train"},
                new Category { Id = 4, Name = "Shopping", Description = "Retail and online shopping", Keywords = "amazon,shopping,retail,store,clothing,electronics,mall"},
                new Category { Id = 5, Name = "Entertainment", Description = "Movies, streaming, games, events", Keywords = "netflix,spotify,cinema,theatre,game,concert,ticket,disney,steam"},
                new Category { Id = 6, Name = "Health", Description = "Pharmacy, gym, medical", Keywords = "pharmacy,drugstore,doctor,hospital,clinic,gym,fitness,dental,vision"},
                new Category { Id = 7, Name = "Utilities", Description = "Hydro, internet, phone, water", Keywords = "hydro,electric,internet,phone,water,gas bill,utility,rogers,bell,telus"},
                new Category { Id = 8, Name = "Travel", Description = "Flights, hotels, car rental", Keywords = "hotel,flight,airbnb,airline,car rental,vacation,resort,booking,expedia"},
                new Category { Id = 9, Name = "Finance", Description = "Banks, insurance, investments", Keywords = "bank,insurance,investment,loan,credit,brokerage,finance,transfer"},
                new Category { Id = 10, Name = "Education", Description = "Tuition, books, courses", Keywords = "university,college,tuition,course,udemy,books,school,learning"},
                new Category { Id = 11, Name = "Subscriptions", Description = "Recurring software & memberships", Keywords = "subscription,membership,saas,annual fee,monthly fee,adobe,microsoft"},
                new Category { Id = 12, Name = "Home", Description = "Rent, mortgage, repairs, furniture", Keywords = "rent,mortgage,furniture,repair,hardware,ikea,home depot,lease"},
                new Category { Id = 13, Name = "Personal Care", Description = "Haircuts, cosmetics, spa", Keywords = "salon,barber,spa,cosmetics,beauty,haircut,skincare,nails"},
                new Category { Id = 14, Name = "Uncategorized", Description = "Could not confidently classify", Keywords = ""}
            );
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MerchantAlias> MerchantAlias { get; set; }
    }
}