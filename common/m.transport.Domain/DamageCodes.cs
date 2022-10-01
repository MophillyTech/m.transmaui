using System.Runtime.Serialization;

namespace m.transport.Domain
{
	[DataContract(Namespace = "http://www.mophilly.com/")]
	public class DamageCodes
	{
		[DataMember]
		public DamageAreaCode[] Areas { get; set; }
		[DataMember]
		public DamageSeverityCode[] Severities { get; set; }
		[DataMember]
		public DamageTypeCode[] Types { get; set; }


		public void Init()
		{
			Severities = new[] {
				new DamageSeverityCode { Code = "1", Description = "Up to 1 in (2.5 cm)" },
				new DamageSeverityCode { Code = "2", Description = "1-3 in (2.5-7.5 cm)" },
				new DamageSeverityCode { Code = "3", Description = "3-6 in (7.5-15 cm)" },
				new DamageSeverityCode { Code = "4", Description = "6-12 in (15-30 cm)" },
				new DamageSeverityCode { Code = "5", Description = "Over 12 in (30 cm)" },
				new DamageSeverityCode { Code = "6", Description = "Missing" },
			};


			Types = new[] {
				new DamageTypeCode { Code = "01", Description = "Bent" },
				new DamageTypeCode { Code = "02", Description = "Broken (except glass)" },
				new DamageTypeCode { Code = "03", Description = "Cut" },
				new DamageTypeCode { Code = "04", Description = "Dented (paint broken)" },
				new DamageTypeCode { Code = "05", Description = "Chipped (except glass/panel edge)" },
				new DamageTypeCode { Code = "06", Description = "Cracked (except glass)" },
				new DamageTypeCode { Code = "07", Description = "Gouged" },
				new DamageTypeCode { Code = "08", Description = "Missing (except molding/emblem/weatherstrip)" },
				new DamageTypeCode { Code = "09", Description = "Scuffed" },
				new DamageTypeCode { Code = "10", Description = "Stained/Soiled - Interior" },
				new DamageTypeCode { Code = "11", Description = "Punctured" },
				new DamageTypeCode { Code = "12", Description = "Scratched (except glass)" },
				new DamageTypeCode { Code = "13", Description = "Torn" },
				new DamageTypeCode { Code = "14", Description = "Dented (paint/chrome not damaged)" },
				new DamageTypeCode { Code = "18", Description = "Molding/Emblem/Weatherstrip Damaged" },
				new DamageTypeCode { Code = "19", Description = "Molding/Emblem/Weatherstrip Loose/Missing" },
				new DamageTypeCode { Code = "20", Description = "Glass Cracked" },
				new DamageTypeCode { Code = "21", Description = "Glass Broken" },
				new DamageTypeCode { Code = "22", Description = "Glass Chipped" },
				new DamageTypeCode { Code = "23", Description = "Glass Scratched" },
				new DamageTypeCode { Code = "24", Description = "Marker Light/Additional Turn Light Damaged" },
				new DamageTypeCode { Code = "25", Description = "Decal/Paint Stripe Damaged" },
				new DamageTypeCode { Code = "29", Description = "Contamination - Exterior" },
				new DamageTypeCode { Code = "30", Description = "Fluid Spillage - Exterior" },
				new DamageTypeCode { Code = "34", Description = "Chipped Panel Edge" },
				new DamageTypeCode { Code = "36", Description = "Incorrect Part/Option not as Invoiced" },
				new DamageTypeCode { Code = "37", Description = "Hardware Damaged" },
				new DamageTypeCode { Code = "38", Description = "Hardware Loose/Missing" },
				//new DamageTypeCode { Code = "99", Description = "PaintDefect/Factory (Ridgefield ONLY)" }
			};

			Areas = new[] {
				new DamageAreaCode { Code = "01", Description = "Antenna/Antenna Base", Location = "FE" },
				new DamageAreaCode { Code = "02", Description = "Battery/Box", Location = "UC+MISC" },
				new DamageAreaCode { Code = "03", Description = "Bumper/Cover/Ext - Front", Location = "FE" },
				new DamageAreaCode { Code = "04", Description = "Bumper/Cover/Ext - Rear", Location = "RE" },
				new DamageAreaCode { Code = "05", Description = "Bumper Guard/Strip - Front", Location = "FE" },
				new DamageAreaCode { Code = "06", Description = "Bumper Guard/Strip - Rear", Location = "RE" },
				new DamageAreaCode { Code = "07", Description = "Door - Right Rear Cargo (truck only)", Location = "RE" },
				new DamageAreaCode { Code = "08", Description = "Door - Left Rear Cargo (truck only)", Location = "RE" },
				new DamageAreaCode { Code = "09", Description = "Door - Right Cargo", Location = "RS" },
				new DamageAreaCode { Code = "10", Description = "Door - Left Front", Location = "LS" },
				new DamageAreaCode { Code = "11", Description = "Door - Left Rear", Location = "LS" },
				new DamageAreaCode { Code = "12", Description = "Door - Right Front", Location = "RS" },
				new DamageAreaCode { Code = "13", Description = "Door - Right Rear", Location = "RS" },
				new DamageAreaCode { Code = "14", Description = "Fender - Left Front", Location = "LS" },
				new DamageAreaCode { Code = "15", Description = "Quarter Panel/Pickup Box - Left", Location = "LS" },
				new DamageAreaCode { Code = "16", Description = "Fender - Right Front", Location = "RS" },
				new DamageAreaCode { Code = "17", Description = "Quarter Panel/Pickup Box - Right", Location = "RS" },
				new DamageAreaCode { Code = "18", Description = "Floor Mats - Front", Location = "RE" },
				new DamageAreaCode { Code = "19", Description = "Floor Mats - Rear", Location = "RE" },
				new DamageAreaCode { Code = "20", Description = "Glass Windshield", Location = "FE" },
				new DamageAreaCode { Code = "21", Description = "Glass Rear", Location = "RE" },
				new DamageAreaCode { Code = "22", Description = "Grille", Location = "FE" },
				new DamageAreaCode { Code = "23", Description = "Accessory Bag/Box", Location = "INT" },
				new DamageAreaCode { Code = "24", Description = "Headlight/Cover/Turn Signal", Location = "FE" },
				new DamageAreaCode { Code = "25", Description = "Lamps - Fog/Driving/Spot Light", Location = "FE" },
				new DamageAreaCode { Code = "26", Description = "Headliner", Location = "INT" },
				new DamageAreaCode { Code = "27", Description = "Hood", Location = "FE" },
				new DamageAreaCode { Code = "28", Description = "Keys", Location = "INT" },
				new DamageAreaCode { Code = "29", Description = "Keyless Remote", Location = "INT" },
				new DamageAreaCode { Code = "30", Description = "Mirror - Left Outside", Location = "LS" },
				new DamageAreaCode { Code = "31", Description = "Mirror - Right Outside", Location = "RS" },
				//new DamageAreaCode { Code = "32", Description = "Entire Vehicle", Location = "EXT" },
				new DamageAreaCode { Code = "33", Description = "Audio/Video Player", Location = "INT" },
				new DamageAreaCode { Code = "34", Description = "TV/DVD Screen", Location = "RE" },
				new DamageAreaCode { Code = "35", Description = "Rocker Panel/Outer Sill - Left", Location = "LS" },
				new DamageAreaCode { Code = "36", Description = "Rocker Panel/Outer Sill - Right", Location = "RS" },
				new DamageAreaCode { Code = "37", Description = "Roof", Location = "RF" },
				new DamageAreaCode { Code = "38", Description = "Running Board/Step - Left (truck only)", Location = "LS" },
				new DamageAreaCode { Code = "39", Description = "Running Board/Step - Right (truck only)", Location = "RS" },
				new DamageAreaCode { Code = "40", Description = "Spare Tire/Wheel", Location = "RE" },
				new DamageAreaCode { Code = "42", Description = "Splash Panel/Spoiler - Front", Location = "FE" },
				new DamageAreaCode { Code = "44", Description = "Gas Tank", Location = "UC+MISC" },
				new DamageAreaCode { Code = "45", Description = "Tail Light/Hardware", Location = "RE" },
				//new DamageAreaCode { Code = "46", Description = "Wheel/Rim", Location = "EXT" },
				//new DamageAreaCode { Code = "47", Description = "Tire Except Spare", Location = "EXT" },
				new DamageAreaCode { Code = "48", Description = "Trim Panel - Left Front", Location = "INT" },
				new DamageAreaCode { Code = "49", Description = "CD Changer Separate Unit", Location = "INT" },
				new DamageAreaCode { Code = "50", Description = "Trim Panel - Right Front", Location = "INT" },
				new DamageAreaCode { Code = "52", Description = "Deck Lid/Tailgate/Hatchback", Location = "RE" },
				new DamageAreaCode { Code = "53", Description = "Sunroof/T-Top", Location = "RF" },
				new DamageAreaCode { Code = "54", Description = "Undercarriage - Other", Location = "UC+MISC" },
				new DamageAreaCode { Code = "55", Description = "Cargo Area - Other", Location = "RE" },
				new DamageAreaCode { Code = "56", Description = "Vinyl/Convertible Top/Tonneau Cover", Location = "RF" },
				new DamageAreaCode { Code = "57", Description = "Wheel Covers/Caps/Rings", Location = "RE" },
				new DamageAreaCode { Code = "58", Description = "Radio Speakers", Location = "INT" },
				new DamageAreaCode { Code = "59", Description = "Wipers - All", Location = "FE" },
				new DamageAreaCode { Code = "61", Description = "Box Interior - Pickup (truck only)", Location = "RE" },
				new DamageAreaCode { Code = "63", Description = "Rails - Truckbed/Lightbar", Location = "UC+MISC" },
				new DamageAreaCode { Code = "64", Description = "Spoiler/Deflector - Rear", Location = "RE" },
				new DamageAreaCode { Code = "65", Description = "Luggage Rack (strips drip rail)", Location = "RF" },
				new DamageAreaCode { Code = "66", Description = "Dash/Instrument Panel", Location = "INT" },
				new DamageAreaCode { Code = "67", Description = "Cigarette Lighter/Ashtray", Location = "INT" },
				new DamageAreaCode { Code = "68", Description = "Carpet - Front", Location = "INT" },
				new DamageAreaCode { Code = "69", Description = "Center Post - Right", Location = "RF" },
				new DamageAreaCode { Code = "70", Description = "Center Post - Left", Location = "RF" },
				new DamageAreaCode { Code = "71", Description = "Corner Post - Front (right or left)", Location = "RF" },
				new DamageAreaCode { Code = "72", Description = "Tire - Left Front", Location = "LS" },
				new DamageAreaCode { Code = "73", Description = "Wheel/Rim - Left Front", Location = "LS" },
				new DamageAreaCode { Code = "74", Description = "Tire - Left Rear", Location = "LS" },
				new DamageAreaCode { Code = "75", Description = "Wheel/Rim - Left Rear", Location = "LS" },
				new DamageAreaCode { Code = "76", Description = "Tire - Right Rear", Location = "RS" },
				new DamageAreaCode { Code = "77", Description = "Wheel/Rim - Right Rear", Location = "RE" },
				new DamageAreaCode { Code = "78", Description = "Tire - Right Front", Location = "RS" },
				new DamageAreaCode { Code = "79", Description = "Wheel/Rim - Right Front", Location = "RS" },
				new DamageAreaCode { Code = "80", Description = "Cowl", Location = "FE" },
				new DamageAreaCode { Code = "81", Description = "Gas/Cap Cover", Location = "UC+MISC" },
				new DamageAreaCode { Code = "82", Description = "Fender - Left Rear", Location = "LS" },
				new DamageAreaCode { Code = "83", Description = "Fender - Right Rear", Location="RS" },
				new DamageAreaCode { Code = "84", Description = "Tools/Jacks/Spare Tire Mount + Lock", Location="RE" }
			};
		}
	}
}

