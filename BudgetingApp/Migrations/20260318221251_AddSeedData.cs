using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BudgetingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Supermarkets, grocery stores, bulk food, and online grocery delivery", "grocery,supermarket,food store,produce,organic,market,bulk,loblaws,metro,sobeys,freshco,food basics,t&t,farm boy,whole foods,longos,safeway,save on foods,maxi,provigo,instacart,goodfood,hellofresh,costco" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Restaurants, cafes, coffee shops, fast food, bars, and food delivery apps", "restaurant,cafe,coffee,takeout,delivery,fast food,pizza,sushi,bistro,bar,pub,diner,grill,doordash,uber eats,skip the dishes,tim hortons,starbucks,mcdonalds,subway,burger king,wendy's,harveys,a&w,pizza pizza,dominos,taco bell,kfc,chipotle,booster juice,freshii,dairy queen" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Gas stations, ride-sharing, taxis, public transit, parking, tolls, and auto services", "gas,fuel,petro,shell,esso,ultramar,uber,lyft,taxi,transit,ttc,presto,stm,octranspo,translink,go transit,via rail,bus,parking,impark,407 etr,toll,car wash,jiffy lube,mr lube,kal tire" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Retail stores, online shopping, clothing, electronics, accessories, and department stores", "amazon,retail,store,clothing,electronics,mall,walmart,winners,marshalls,old navy,gap,h&m,zara,uniqlo,aritzia,lululemon,sport chek,best buy,staples,coach,michael kors,roots,indigo,chapters,ebay,etsy,wayfair,dollarama,canadian tire,nike,adidas,aldo,foot locker,simons" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Streaming services, movies, concerts, gaming, sports events, and recreational activities", "netflix,spotify,cinema,theatre,gaming,concert,ticket,disney,steam,crave,apple tv,youtube,prime video,dazn,cineplex,ticketmaster,stubhub,xbox,playstation,nintendo,epic games,audible,escape room,bowling,casino" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Pharmacies, medical clinics, hospitals, gyms, fitness studios, dental, and vision care", "pharmacy,drugstore,doctor,hospital,clinic,gym,fitness,dental,vision,shoppers drug mart,rexall,pharmasave,lifelabs,dynacare,goodlife,anytime fitness,ymca,orangetheory,f45,physiotherapy,massage,optometrist,lenscrafters,clearly" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Electricity, natural gas, water, internet, and cell phone providers", "hydro,electric,internet,phone,water,utility,rogers,bell,telus,fido,koodo,public mobile,shaw,videotron,eastlink,teksavvy,hydro one,toronto hydro,enbridge,fortis,bc hydro,hydro quebec,nova scotia power,atco" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Flights, hotels, car rental, vacation packages, and travel booking platforms", "hotel,flight,airbnb,airline,car rental,vacation,resort,booking,expedia,air canada,westjet,porter,swoop,sunwing,air transat,marriott,hilton,hyatt,best western,enterprise,hertz,avis,priceline,hotels.com,kayak,trivago" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Banks, credit unions, insurance, investments, crypto, and money transfers", "bank,insurance,investment,loan,credit,brokerage,transfer,paypal,wealthsimple,questrade,coinbase,western union,manulife,sunlife,intact,belairdirect,aviva,desjardins,tangerine,simplii,eq bank,interac,stripe" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "University tuition, textbooks, online courses, and tutoring", "university,college,tuition,course,udemy,coursera,skillshare,books,school,learning,linkedin learning,pluralsight,masterclass,duolingo,chegg,campus store,bookstore,student" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Recurring software subscriptions, SaaS tools, and professional memberships", "subscription,membership,saas,annual fee,adobe,microsoft,google one,dropbox,icloud,notion,slack,zoom,1password,nordvpn,canva,github,openai,anthropic,grammarly,shopify,squarespace,mailchimp,hootsuite,figma,cursor" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Rent, mortgage, furniture, appliances, hardware stores, and home improvement", "rent,mortgage,furniture,repair,hardware,ikea,home depot,rona,lowes,canadian tire home,leon's,the brick,sleep country,endy,casper,wayfair furniture,structube,article,restoration hardware,pottery barn,renovation,contractor,lease" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Hair salons, barbershops, spas, cosmetics, and personal grooming", "salon,barber,spa,cosmetics,beauty,haircut,skincare,nails,sephora,mac cosmetics,lush,bath body works,the body shop,supercuts,great clips,sport clips,hand and stone,massage envy,ulta,morphe" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.InsertData(
                table: "MerchantAlias",
                columns: new[] { "Id", "Category", "RawName" },
                values: new object[,]
                {
                    { 1, "Transport", "Uber" },
                    { 2, "Transport", "Lyft" },
                    { 3, "Dining", "Uber Eats" },
                    { 4, "Dining", "Doordash" },
                    { 5, "Dining", "Skip The Dishes" },
                    { 6, "Shopping", "Coach" },
                    { 7, "Shopping", "Roots" },
                    { 8, "Shopping", "Sail" },
                    { 9, "Shopping", "Aldo" },
                    { 10, "Shopping", "Simons" },
                    { 11, "Entertainment", "Netflix" },
                    { 12, "Entertainment", "Spotify" },
                    { 13, "Entertainment", "Crave" },
                    { 14, "Entertainment", "Dazn" },
                    { 15, "Utilities", "Fido" },
                    { 16, "Utilities", "Koodo" },
                    { 17, "Utilities", "Shaw" },
                    { 18, "Utilities", "Videotron" },
                    { 19, "Utilities", "Teksavvy" },
                    { 20, "Finance", "Wealthsimple" },
                    { 21, "Finance", "Questrade" },
                    { 22, "Finance", "Ndax" },
                    { 23, "Finance", "Newton" },
                    { 24, "Subscriptions", "Notion" },
                    { 25, "Subscriptions", "Figma" },
                    { 26, "Subscriptions", "Canva" },
                    { 27, "Subscriptions", "Github" },
                    { 28, "Subscriptions", "Openai" },
                    { 29, "Subscriptions", "Anthropic" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "MerchantAlias",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Supermarkets and food stores", "grocery,supermarket,food,produce,organic,market" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Restaurants, cafes, and takeout", "restaurant,cafe,coffee,takeout,delivery,fast food,pizza,sushi,bistro" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Gas, transit, rideshare, parking", "gas,fuel,uber,lyft,taxi,transit,subway,bus,parking,toll,train" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Retail and online shopping", "amazon,shopping,retail,store,clothing,electronics,mall" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Movies, streaming, games, events", "netflix,spotify,cinema,theatre,game,concert,ticket,disney,steam" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Pharmacy, gym, medical", "pharmacy,drugstore,doctor,hospital,clinic,gym,fitness,dental,vision" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Hydro, internet, phone, water", "hydro,electric,internet,phone,water,gas bill,utility,rogers,bell,telus" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Flights, hotels, car rental", "hotel,flight,airbnb,airline,car rental,vacation,resort,booking,expedia" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Banks, insurance, investments", "bank,insurance,investment,loan,credit,brokerage,finance,transfer" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Tuition, books, courses", "university,college,tuition,course,udemy,books,school,learning" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Recurring software & memberships", "subscription,membership,saas,annual fee,monthly fee,adobe,microsoft" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Rent, mortgage, repairs, furniture", "rent,mortgage,furniture,repair,hardware,ikea,home depot,lease" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Description", "Keywords" },
                values: new object[] { "Haircuts, cosmetics, spa", "salon,barber,spa,cosmetics,beauty,haircut,skincare,nails" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
