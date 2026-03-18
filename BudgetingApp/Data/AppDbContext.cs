using BudgetingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetingApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MerchantAlias>()
                .HasIndex(m => m.RawName)
                .IsUnique();
            modelBuilder.Entity<Category>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("NOW()");

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Groceries", Description = "Supermarkets, grocery stores, bulk food, and online grocery delivery", Keywords = "grocery,supermarket,food store,produce,organic,market,bulk,loblaws,metro,sobeys,freshco,food basics,t&t,farm boy,whole foods,longos,safeway,save on foods,maxi,provigo,instacart,goodfood,hellofresh,costco", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Category { Id = 2, Name = "Dining", Description = "Restaurants, cafes, coffee shops, fast food, bars, and food delivery apps", Keywords = "restaurant,cafe,coffee,takeout,delivery,fast food,pizza,sushi,bistro,bar,pub,diner,grill,doordash,uber eats,skip the dishes,tim hortons,starbucks,mcdonalds,subway,burger king,wendy's,harveys,a&w,pizza pizza,dominos,taco bell,kfc,chipotle,booster juice,freshii,dairy queen", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Category { Id = 3, Name = "Transport", Description = "Gas stations, ride-sharing, taxis, public transit, parking, tolls, and auto services", Keywords = "gas,fuel,petro,shell,esso,ultramar,uber,lyft,taxi,transit,ttc,presto,stm,octranspo,translink,go transit,via rail,bus,parking,impark,407 etr,toll,car wash,jiffy lube,mr lube,kal tire", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Category { Id = 4, Name = "Shopping", Description = "Retail stores, online shopping, clothing, electronics, accessories, and department stores", Keywords = "amazon,retail,store,clothing,electronics,mall,walmart,winners,marshalls,old navy,gap,h&m,zara,uniqlo,aritzia,lululemon,sport chek,best buy,staples,coach,michael kors,roots,indigo,chapters,ebay,etsy,wayfair,dollarama,canadian tire,nike,adidas,aldo,foot locker,simons", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Category { Id = 5, Name = "Entertainment", Description = "Streaming services, movies, concerts, gaming, sports events, and recreational activities", Keywords = "netflix,spotify,cinema,theatre,gaming,concert,ticket,disney,steam,crave,apple tv,youtube,prime video,dazn,cineplex,ticketmaster,stubhub,xbox,playstation,nintendo,epic games,audible,escape room,bowling,casino", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Category { Id = 6, Name = "Health", Description = "Pharmacies, medical clinics, hospitals, gyms, fitness studios, dental, and vision care", Keywords = "pharmacy,drugstore,doctor,hospital,clinic,gym,fitness,dental,vision,shoppers drug mart,rexall,pharmasave,lifelabs,dynacare,goodlife,anytime fitness,ymca,orangetheory,f45,physiotherapy,massage,optometrist,lenscrafters,clearly", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Category { Id = 7, Name = "Utilities", Description = "Electricity, natural gas, water, internet, and cell phone providers", Keywords = "hydro,electric,internet,phone,water,utility,rogers,bell,telus,fido,koodo,public mobile,shaw,videotron,eastlink,teksavvy,hydro one,toronto hydro,enbridge,fortis,bc hydro,hydro quebec,nova scotia power,atco", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Category { Id = 8, Name = "Travel", Description = "Flights, hotels, car rental, vacation packages, and travel booking platforms", Keywords = "hotel,flight,airbnb,airline,car rental,vacation,resort,booking,expedia,air canada,westjet,porter,swoop,sunwing,air transat,marriott,hilton,hyatt,best western,enterprise,hertz,avis,priceline,hotels.com,kayak,trivago", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Category { Id = 9, Name = "Finance", Description = "Banks, credit unions, insurance, investments, crypto, and money transfers", Keywords = "bank,insurance,investment,loan,credit,brokerage,transfer,paypal,wealthsimple,questrade,coinbase,western union,manulife,sunlife,intact,belairdirect,aviva,desjardins,tangerine,simplii,eq bank,interac,stripe", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Category { Id = 10, Name = "Education", Description = "University tuition, textbooks, online courses, and tutoring", Keywords = "university,college,tuition,course,udemy,coursera,skillshare,books,school,learning,linkedin learning,pluralsight,masterclass,duolingo,chegg,campus store,bookstore,student", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Category { Id = 11, Name = "Subscriptions", Description = "Recurring software subscriptions, SaaS tools, and professional memberships", Keywords = "subscription,membership,saas,annual fee,adobe,microsoft,google one,dropbox,icloud,notion,slack,zoom,1password,nordvpn,canva,github,openai,anthropic,grammarly,shopify,squarespace,mailchimp,hootsuite,figma,cursor", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Category { Id = 12, Name = "Home", Description = "Rent, mortgage, furniture, appliances, hardware stores, and home improvement", Keywords = "rent,mortgage,furniture,repair,hardware,ikea,home depot,rona,lowes,canadian tire home,leon's,the brick,sleep country,endy,casper,wayfair furniture,structube,article,restoration hardware,pottery barn,renovation,contractor,lease", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Category { Id = 13, Name = "Personal Care", Description = "Hair salons, barbershops, spas, cosmetics, and personal grooming", Keywords = "salon,barber,spa,cosmetics,beauty,haircut,skincare,nails,sephora,mac cosmetics,lush,bath body works,the body shop,supercuts,great clips,sport clips,hand and stone,massage envy,ulta,morphe", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Category { Id = 14, Name = "Uncategorized", Description = "Could not confidently classify", Keywords = "", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );

            // Aliases are only seeded for merchants the model can't classify from name alone:
            // ambiguous brand names, acronyms, and short tokens with no semantic content.
            // All descriptive merchant names (e.g. "Home Depot", "Pizza Pizza") are handled
            // by the model via keyword matching without needing an explicit alias.
            modelBuilder.Entity<MerchantAlias>().HasData(
                // Ride-share — opaque brand names with no transport semantics
                new MerchantAlias { Id = 1, RawName = "Uber", Category = "Transport" },
                new MerchantAlias { Id = 2, RawName = "Lyft", Category = "Transport" },

                // Food delivery — "Uber" alone (post-normalisation) would hit Transport above;
                // "Uber Eats" needs its own entry to route correctly to Dining
                new MerchantAlias { Id = 3, RawName = "Uber Eats", Category = "Dining" },
                new MerchantAlias { Id = 4, RawName = "Doordash", Category = "Dining" },
                new MerchantAlias { Id = 5, RawName = "Skip The Dishes", Category = "Dining" },

                // Ambiguous retail — "Coach" (coaching/transit), "Roots" (nature), "Sail" (sailing)
                new MerchantAlias { Id = 6, RawName = "Coach", Category = "Shopping" },
                new MerchantAlias { Id = 7, RawName = "Roots", Category = "Shopping" },
                new MerchantAlias { Id = 8, RawName = "Sail", Category = "Shopping" },
                new MerchantAlias { Id = 9, RawName = "Aldo", Category = "Shopping" },
                new MerchantAlias { Id = 10, RawName = "Simons", Category = "Shopping" },

                // Streaming — consumed content, not SaaS; would otherwise score high on Subscriptions
                new MerchantAlias { Id = 11, RawName = "Netflix", Category = "Entertainment" },
                new MerchantAlias { Id = 12, RawName = "Spotify", Category = "Entertainment" },
                new MerchantAlias { Id = 13, RawName = "Crave", Category = "Entertainment" },
                new MerchantAlias { Id = 14, RawName = "Dazn", Category = "Entertainment" },

                // Telecom with no semantic signal in their names
                new MerchantAlias { Id = 15, RawName = "Fido", Category = "Utilities" },
                new MerchantAlias { Id = 16, RawName = "Koodo", Category = "Utilities" },
                new MerchantAlias { Id = 17, RawName = "Shaw", Category = "Utilities" },
                new MerchantAlias { Id = 18, RawName = "Videotron", Category = "Utilities" },
                new MerchantAlias { Id = 19, RawName = "Teksavvy", Category = "Utilities" },

                // Fintech/crypto — names with no financial semantics
                new MerchantAlias { Id = 20, RawName = "Wealthsimple", Category = "Finance" },
                new MerchantAlias { Id = 21, RawName = "Questrade", Category = "Finance" },
                new MerchantAlias { Id = 22, RawName = "Ndax", Category = "Finance" },
                new MerchantAlias { Id = 23, RawName = "Newton", Category = "Finance" },

                // SaaS tools that score ambiguously against Education or Entertainment
                new MerchantAlias { Id = 24, RawName = "Notion", Category = "Subscriptions" },
                new MerchantAlias { Id = 25, RawName = "Figma", Category = "Subscriptions" },
                new MerchantAlias { Id = 26, RawName = "Canva", Category = "Subscriptions" },
                new MerchantAlias { Id = 27, RawName = "Github", Category = "Subscriptions" },
                new MerchantAlias { Id = 28, RawName = "Openai", Category = "Subscriptions" },
                new MerchantAlias { Id = 29, RawName = "Anthropic", Category = "Subscriptions" }
            );
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MerchantAlias> MerchantAlias { get; set; }
    }
}