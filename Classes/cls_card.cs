using System.Collections.Generic;
using System.Linq;
using JsonFlatFileDataStore;
using Newtonsoft.Json;
using Discord;

namespace zgrl.Classes {
    public enum CardLegality {
        BLUE = 0,
        GREEN = 1,
        YELLOW = 2,
        RED = 3,
        INVALID = 4
    }

    public enum CardType {
        Utility = 0,
        Defensive = 1,
        Offensive = 2,
        TechAttack = 3,
        DefensiveOffensive = 4,
        INVALID = 5
    }

    public enum CardConditionOptions {
        Adaptability,
        Skill,
        Intelligence,
        Agility,
        Armor,
        Attack,
        Damage,
        Health,
        Hull,
        Speed,
        Tech,
        INVALID
    }

    public class CardCondition {
        public CardConditionOptions cardCondition { get; set;}
        public int value { get; set;}

        public override string ToString() {
            return value + " " + CardCondition.cardConditionString(cardCondition);
        }

        public bool check (Car car, Racer racer) {
            switch (cardCondition) {
                case CardConditionOptions.Adaptability:
                return racer.adaptability >= value;
                case CardConditionOptions.Agility:
                return car.Agility >= value;
                case CardConditionOptions.Armor:
                return car.Armor >= value;
                case CardConditionOptions.Attack:
                return car.Attack >= value;
                case CardConditionOptions.Damage:
                return car.Damage >= value;
                case CardConditionOptions.Health:
                return car.Health >= value;
                case CardConditionOptions.Hull:
                return car.Hull >= value;
                case CardConditionOptions.Intelligence:
                return racer.intel >= value;
                case CardConditionOptions.Skill:
                return racer.skill >= value;
                case CardConditionOptions.Speed:
                return car.Speed >= value;
                case CardConditionOptions.Tech:
                return car.Tech >= value;
                default:
                return false;
            }
        }

        public CardCondition(CardConditionOptions option, int val) {
            cardCondition = option;
            value=val;
        }

        public static string cardConditionString(CardConditionOptions cardCondition) {
            switch(cardCondition) {
                case CardConditionOptions.Adaptability:
                    return "Adaptability";
                case CardConditionOptions.Skill:
                    return "Skill";
                case CardConditionOptions.Intelligence:
                    return "Intelligence";
                case CardConditionOptions.Agility:
                    return "Agility";
                case CardConditionOptions.Armor:
                    return "Armor";
                case CardConditionOptions.Attack:
                    return "Attack";
                case CardConditionOptions.Damage:
                    return "Damage";
                case CardConditionOptions.Health:
                    return "Health";
                case CardConditionOptions.Hull:
                    return "Hull";
                case CardConditionOptions.Speed:
                    return "Speed";
                case CardConditionOptions.Tech:
                    return "Tech";
                default:
                    return "INVALID";
            }
        }

        public static CardConditionOptions stringToCardCondition(string input) {
            switch(input.ToLowerInvariant()) {
                case "adaptability":
                    return CardConditionOptions.Adaptability;
                case "skill":
                    return CardConditionOptions.Skill;
                case "intelligence":
                    return CardConditionOptions.Intelligence;
                case "agility":
                    return CardConditionOptions.Agility;
                case "armor":
                    return CardConditionOptions.Armor;
                case "attack":
                    return CardConditionOptions.Attack;
                case "damage":
                    return CardConditionOptions.Damage;
                case "health":
                    return CardConditionOptions.Health;
                case "hull":
                    return CardConditionOptions.Hull;
                case "speed":
                    return CardConditionOptions.Speed;
                case "tech":
                    return CardConditionOptions.Tech;
                default:
                    return CardConditionOptions.INVALID;
            }
        }
    }

    public partial class Card {
        [JsonProperty ("ID")]
        public int ID { get; set; }
        [JsonProperty ("count")]
        public int count { get; set; } = 1;

        [JsonProperty ("title")]
        public string title { get; set; } = "";
        [JsonProperty("customtitle")]
        public string customTitle { get; set; }
        [JsonProperty("acronym")]
        public string acronym { get; set;}
        [JsonProperty("customAcronym")]
        public string customAcronym { get; set;}

        [JsonProperty ("description")] 
        public string description { get; set; } = "";
        [JsonProperty("customLore")]
        public string customLore {get; set;}
        [JsonProperty("img")]
        public string img { get; set; } = "";

        [JsonProperty("success")]
        public string success { get; set; } = "";

        [JsonProperty("failure")]
        public string failure { get; set; } = "";

        [JsonProperty("cardLegality")]
        public CardLegality cardLegality { get; set; } = CardLegality.INVALID;
        [JsonProperty("cardType")]
        public CardType cardType { get; set; } = CardType.INVALID;
        [JsonProperty("conditions")]
        public List<CardCondition> conditions { get; set; } = new List<CardCondition>();

        public Embed embed() {
            var embed = new EmbedBuilder();

            embed.WithTitle(embedTitle());
            embed.WithDescription(completeDescription());
            embed.WithThumbnailUrl(img);
            embed.AddField("ID",ID.ToString(),true);
            embed.AddField("Card Legality",Card.cardLegalityString(cardLegality), true);
            embed.AddField("Card Type", Card.cardTypeString(cardType), true);
            if (customLore != null) embed.AddField("Lore", customLore, true);
            switch(cardLegality) {
                case CardLegality.BLUE:
                    embed.WithColor(Color.Blue);
                break;
                case CardLegality.RED:
                    embed.WithColor(Color.Red);
                break;
                case CardLegality.YELLOW:
                    embed.WithColor(Color.Gold);
                break;
                case CardLegality.GREEN:
                    embed.WithColor(Color.Green);
                break;
            }
            if (conditions.Count > 0) {
                var strs = new List<string>();
                foreach (var condition in conditions) {
                    strs.Add(condition.ToString());
                }
                embed.AddField("Conditions", string.Join(System.Environment.NewLine, strs));
            }

            return embed.Build();
        }

        public string completeDescription() {
            var rtrner = description;
            if (!success.Equals("")) {
                rtrner += System.Environment.NewLine + "**Success:**" + System.Environment.NewLine + success;
            }
            if (!failure.Equals("")) {
                rtrner += System.Environment.NewLine + "**Failure:**" + System.Environment.NewLine + failure;
            }
            if (conditions.Count > 0) {
                var strs = new List<string>();
                foreach (var condition in conditions) {
                    strs.Add(condition.ToString());
                }
                rtrner += System.Environment.NewLine + "**Conditions:**" + System.Environment.NewLine + string.Join(System.Environment.NewLine, strs);
            }
            return rtrner;
        }

        public bool update(Dictionary<string, string> inputs, out string error) {
            error = "";
            foreach (string token in inputs.Keys) {
                if (validUpdateCardKeys.Contains(token.ToLowerInvariant())) {
                    switch(token.ToLowerInvariant()) {
                        case "title":
                            customTitle = inputs[token];
                        break;
                        case "lore":
                            customLore = inputs[token];
                        break;
                        case "img":
                            img = inputs[token];
                        break;
                        case "acronym":
                            customAcronym = inputs[token];
                        break;
                        default:
                            error = error + System.Environment.NewLine + "Unrecognized Key: " + token + " Value: " + inputs[token];
                        break;
                    }
                } else {
                    var condition = CardCondition.stringToCardCondition(token.ToLowerInvariant());
                    if (condition != CardConditionOptions.INVALID) {
                        if (int.TryParse(inputs[token], out int value)) {
                            conditions.Add(new CardCondition(condition, value));
                        } else {
                            error = "Card condition (" + condition + ") had no valid value assigned.";
                            return false;
                        }
                    } else {
                        error = error + System.Environment.NewLine + "Unrecognized Key: " + token + " Value: " + inputs[token];
                    }
                }
            }
            return true;
        }

        public bool updateNewCard(Dictionary<string, string> inputs, out string error) {
            error = "";
            foreach (string token in inputs.Keys) {
                if (validNewCardKeys.Contains(token.ToLowerInvariant())) {
                    switch(token.ToLowerInvariant()) {
                        case "title":
                            title = inputs[token];
                        break;
                        case "description":
                            description = inputs[token];
                        break;
                        case "lore":
                            customLore = inputs[token];
                        break;
                        case "success":
                            success = inputs[token];
                        break;
                        case "failure":
                            failure = inputs[token];
                        break;
                        case "legality":
                            cardLegality = Card.stringToCardLegality(inputs[token]);
                            if (cardLegality == CardLegality.INVALID) {
                                error = "Card Legality input valid. Received: " + inputs[token];
                                return false;
                            }
                        break;
                        case "type":
                            cardType = Card.stringToCardType(inputs[token]);
                            if (cardType == CardType.INVALID) {
                                error = "Card Type input valid. Received: " + inputs[token];
                                return false;
                            }
                        break;
                        case "acronym":
                            acronym = inputs[token];
                        break;
                    }
                } else {
                    var condition = CardCondition.stringToCardCondition(token.ToLowerInvariant());
                    if (condition != CardConditionOptions.INVALID) {
                        if (int.TryParse(inputs[token], out int value)) {
                            conditions.Add(new CardCondition(condition, value));
                        } else {
                            error = "Card condition (" + condition + ") had no valid value assigned.";
                            return false;
                        }
                    } else {
                        error = error + System.Environment.NewLine + "Unrecognized Key: " + token + " Value: " + inputs[token];
                    }
                }
            }
            return true;
        }

    }
    public partial class Card
    {
        public static string[] validUpdateCardKeys = {"lore", "title", "img", "acronym"};
        public static string[] validNewCardKeys = {"title", "description", "lore", "success", "failure", "legality", "type", "acronym"};
        public static Card[] FromJson(string json) => JsonConvert.DeserializeObject<Card[]>(json, Converter.Settings);

        public static List<CardLegality> CardLegalities = new List<CardLegality>() {
            CardLegality.BLUE,
            CardLegality.YELLOW,
            CardLegality.RED,
            CardLegality.GREEN
        };

        public static List<CardType> CardTypes = new List<CardType>() {
            CardType.Defensive,
            CardType.Offensive,
            CardType.DefensiveOffensive,
            CardType.TechAttack,
            CardType.Utility
        };

        public static string cardLegalityString(CardLegality cardLegality) {
            switch (cardLegality) {
                case CardLegality.BLUE:
                    return "Blue";
                case CardLegality.YELLOW:
                    return "Yellow";
                case CardLegality.RED:
                    return "Red";
                case CardLegality.GREEN:
                    return "Green";
                default:
                    return "INVALID";
            }
        }

        public static CardLegality stringToCardLegality(string input) {
            switch(input.ToLowerInvariant()) {
                case "blue":
                    return CardLegality.BLUE;
                case "red":
                    return CardLegality.RED;
                case "yellow":
                    return CardLegality.YELLOW;
                case "green":
                    return CardLegality.GREEN;
                default:
                    return CardLegality.INVALID;
            }
        }

        public static string cardTypeString(CardType cardType) {
            switch(cardType) {
                case CardType.Defensive:
                    return "Defensive";
                case CardType.DefensiveOffensive:
                    return "Defensive/Offensive";
                case CardType.Offensive:
                    return "Offensive";
                case CardType.TechAttack:
                    return "Tech Attack";
                case CardType.Utility:
                    return "Utility";
                default:
                    return "INVALID";
            }
        }

        public static CardType stringToCardType(string input) {
            switch(input.ToLowerInvariant()) {
                case "defensive":
                    return CardType.Defensive;
                case "defensive/offensive":
                    return CardType.DefensiveOffensive;
                case "offensive":
                    return CardType.Offensive;
                case "tech attack":
                    return CardType.TechAttack;
                case "utility":
                    return CardType.Utility;
                default:
                    return CardType.INVALID;
            }
        }

        public override string ToString() {
            if (customAcronym != null) {
                return "**" + customAcronym + "**(Legality: " + Card.cardLegalityString(cardLegality) + ") *Type: " + Card.cardTypeString(cardType) + "* Description: " + description;
            } else if (acronym != null) {
                return "**" + acronym + "**(Legality: " + Card.cardLegalityString(cardLegality) + ") *Type: " + Card.cardTypeString(cardType) + "* Description: " + description;
            } else if (customTitle is null) {
                return "**" + title + "** (Legality: " + Card.cardLegalityString(cardLegality) + ") *Type: " + Card.cardTypeString(cardType) + "* Description: " + description;               
            } else {
                return "**" + customTitle + "** (Legality: " + Card.cardLegalityString(cardLegality) + ") *Type: " + Card.cardTypeString(cardType) + "* Description: " + description; 
            }
        }

        public string embedTitle() {
            if (customTitle is null) {
                return "ID " + ID + ": **" + title + "** (Legality: " + Card.cardLegalityString(cardLegality) + ") *Type: " + Card.cardTypeString(cardType) + "*";
            } else {
                return "ID " + ID +": **" + customTitle + "** (Legality: " + Card.cardLegalityString(cardLegality) + ") *Type: " + Card.cardTypeString(cardType) + "*";
            }
        }

        public static List<Card> get_card (string location) {
            var store = new DataStore (location);

            // Get employee collection
            var rtrner = store.GetCollection<Card> ().AsQueryable ().ToList();
            store.Dispose();
            return rtrner;
        }

        public static Card get_card (string location, int id) {
            var store = new DataStore (location);

            // Get employee collection
            var rtrner = store.GetCollection<Card> ().AsQueryable ().FirstOrDefault (e => e.ID == id);
            store.Dispose();
            return rtrner;
        }

        public static Card get_card (string location, string name) {
            var store = new DataStore (location);

            // Get employee collection
            var rtrner = store.GetCollection<Card> ().AsQueryable ().FirstOrDefault (e => e.title == name);
            store.Dispose();
            return rtrner;
        }

        public static void insert_card (string location, Card card) {
            var store = new DataStore (location);

            // Get employee collection
            store.GetCollection<Card> ().InsertOneAsync (card);

            store.Dispose();
        }

        public static void update_card (string location, Card card) {
            var store = new DataStore (location);

            store.GetCollection<Card> ().ReplaceOneAsync (e => e.ID == card.ID, card);
            store.Dispose();
        }

        public static void delete_card (string location, Card card) {
            var store = new DataStore (location);

            store.GetCollection<Card> ().DeleteOne (e => e.ID == card.ID);
            store.Dispose();
        }
    }

}