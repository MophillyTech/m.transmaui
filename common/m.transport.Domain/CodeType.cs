using System;
using System.Collections.Generic;

namespace m.transport.Domain
{
	public sealed class CodeType {

		private readonly String name;

		public static readonly CodeType DriverExpense = new CodeType ("DriverExpenseType");    
		public static readonly CodeType DriverExpenseDefault = new CodeType ("DriverExpenseDefault");  
		public static readonly CodeType DamageNoPhotoReason = new CodeType ("DamageNoPhotoReason");  
		public static readonly CodeType MobileLocationCode = new CodeType ("MobileDropLocationCode");
        public static readonly CodeType SafeDeliveryPromptResponse = new CodeType("SafeDeliveryPromptResponse");

        private static List<CodeType> codeTypeList;
		private static List<string> codeTypeListName;

		public static List<CodeType> CodeTypeList { 
			get{
				if (codeTypeList == null) {
					codeTypeList = new List<CodeType> ();
					codeTypeList.Add (DriverExpense);
					codeTypeList.Add (DriverExpenseDefault);
					codeTypeList.Add (DamageNoPhotoReason);
					codeTypeList.Add (MobileLocationCode);
                    codeTypeList.Add (SafeDeliveryPromptResponse);
                }
					
				return codeTypeList;
			}
		}

		private CodeType(String name){
			this.name = name;
		}

		public override String ToString(){
			return name;
		}

		public static List<string> CodeTypeNameList{
			get{

				if (codeTypeListName == null) {
					codeTypeListName = new List<string> ();
					foreach(CodeType d in CodeTypeList){
						//added single quote so that it can be use to query db directly in webservice
						codeTypeListName.Add ("'" + d.name  + "'");
					}

				}
					
				return codeTypeListName;
			}

		}

	}
}

