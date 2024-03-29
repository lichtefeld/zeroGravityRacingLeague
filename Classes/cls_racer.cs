using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.Commands;
using JsonFlatFileDataStore;

namespace zgrl.Classes {
    public class Racer {
        public static string[] validKeys = {"name", "sponsor", "desc", "img", "adapt", "skill", "int"};
        public int ID { get; set; }
        public ulong player_discord_id { get; set; }
        public ulong server_discord_id { get ; set; }
        public string name { get; set; } = "";
        public string faction { get; set; } = "";
        public string descr { get; set; } = "No Description";
        public string img { get; set; } = "";
        public bool inGame { get; set; } = false;
        public int adaptability { get; set; } = 1;
        public int skill { get; set; } = 1;
        public int intel { get; set; } = 1;
        public List<Ability> abilities = new List<Ability>();
        public List<Car> cars = new List<Car>();
        public Dictionary<int,Dictionary<CardLegality,long>> carToDeckLegalityToDeckID { get; set; } = new Dictionary<int, Dictionary<CardLegality,long>>();

        //Variables Used for Game Mechanics:
        public List<Deck> decks = new List<Deck>();

        public Embed embed(int i, SocketCommandContext Context) {
            var embed = new EmbedBuilder();

            embed.Title = "Pilot Name: " + name;
            embed.WithDescription(descr);
            embed.WithThumbnailUrl(img);
            embed.AddField("ID",ID.ToString(),true);
            embed.AddField("Sponsor",faction, true);
            embed.AddField("Attributes","Adaptability: " + adaptability + System.Environment.NewLine + "Intelligence: " + intel + System.Environment.NewLine + "Skill: " + skill, true);
            foreach (Classes.Ability ability in abilities) {
                embed.AddField("Ability:" + ability.Title, ability.Description, true);
            }
            foreach (Classes.Car car in cars) {
                embed.AddField(car.racerEmbed(), car.description, true);
            }
            if( i < 0 ) {
                embed.AddField("Player",Context.User.Mention,true);
            } else {
                var usr = Context.Guild.GetUser(player_discord_id);
                embed.AddField("Player",usr.Mention,true);
            }

            return embed.Build();
        }

        public bool update(Dictionary<string, string> inputs, out string error) {
            int result;
            foreach(var token in validKeys) {
                if (inputs.ContainsKey(token.ToLowerInvariant())){
                    switch(token.ToLowerInvariant()) {
                        case "name":
                            name = inputs[token];
                        break;
                        case "sponsor":
                            faction = inputs[token];
                        break;
                        case "desc":
                            descr = inputs[token];
                        break;
                        case "img":
                            img = inputs[token];
                        break;
                        case "adapt":
                            if (int.TryParse(inputs[token], out result))
                            {
                                adaptability = result;
                            } else {
                                error = "You didn't provide a valid number for your adaptability level";
                                return false;
                            }
                        break;
                        case "skill":
                            if (int.TryParse(inputs[token], out result))
                            {
                                skill = result;
                            } else {
                                error = "You didn't provide a valid number for your skill level";
                                return false;
                            }
                        break;
                        case "int":
                            if (int.TryParse(inputs[token], out result))
                            {
                                intel = result;
                            } else {
                                error = "You didn't provide a valid number for your intelligence level";
                                return false;
                            }
                        break;
                    }
                }
            }

            // Logic to check that the skills don't exceed 12 points total (of cost)
            var count = 0;

            for (int i = 2; count < 12; i++) {
                var less = true;
                if (i <= adaptability) {
                    count += i;
                    less = less && false;
                } else {
                    less = less && true;
                }
                if (i <= skill) {
                    count += i;
                    less = less && false;
                } else {
                    less = less && true;
                }
                if (i <= intel) {
                    count += i;
                    less = less && false;
                } else {
                    less = less && true;
                }
                if (less) {
                    error = "Your total spend in attributes is less than the alotted amount of 12. Got:" + count;
                    return false;
                }
            }

            if (count > 12) {
                error = "Your total spend in attributes is greater than the alotted amount of 12. Got: " + count;
                return false;
            }
            error = "";
            return true;

        }

        public void reset() {
            inGame = false;
        }

        public string nameID() {
            return this.name + " (" + this.ID + ")";
        }


        public static List<Racer> get_racer () {
            var store = new DataStore ("racer.json");

            var rtner = store.GetCollection<Racer> ().AsQueryable ().ToList();

            store.Dispose();

            // Get employee collection
            return rtner;
        }

        public static Racer get_racer (int id) {
            var store = new DataStore ("racer.json");

            // Get employee collection
            var rtner = store.GetCollection<Racer> ().AsQueryable ().FirstOrDefault (e => e.ID == id);
            store.Dispose();

            // Get employee collection
            return rtner;
        }

        public static Racer get_racer (string name) {
            var store = new DataStore ("racer.json");

            // Get employee collection
            var rtner = store.GetCollection<Racer> ().AsQueryable ().FirstOrDefault (e => e.name == name);
            store.Dispose();

            // Get employee collection
            return rtner;
        }

        public static Racer get_racer (ulong player_id, ulong server_id) {
            var store = new DataStore ("racer.json");

            // Get employee collection
            var rtner = store.GetCollection<Racer> ().AsQueryable ().FirstOrDefault (e => e.player_discord_id == player_id && e.server_discord_id == server_id);
            store.Dispose();

            // Get employee collection
            return rtner;
        }

        public static void insert_racer (Racer racer) {
            var store = new DataStore ("racer.json");

            // Get employee collection
            store.GetCollection<Racer> ().InsertOneAsync (racer);

            store.Dispose();
        }

        public static void update_racer (Racer racer) {
            var store = new DataStore ("racer.json");

            store.GetCollection<Racer> ().ReplaceOne (e => e.ID == racer.ID, racer);
            store.Dispose();
        }

        public static void replace_racer(Racer racer) {
            var store = new DataStore ("racer.json");

            store.GetCollection<Racer> ().ReplaceOne (e => e.ID == racer.ID, racer);
            store.Dispose();
        }

        public static void delete_racer (Racer racer) {
            var store = new DataStore ("racer.json");

            store.GetCollection<Racer> ().DeleteOne (e => e.ID == racer.ID);
            store.Dispose();
        }

    }

}