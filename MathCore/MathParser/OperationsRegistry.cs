//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Reflection;
//using System.Xml;
//using System.Xml.Schema;
//using ELW.Library.Math.Calculators;

//namespace ELW.Library.Math
//{
//    /// <summary>Регистратор операций</summary>
//    public sealed class OperationsRegistry
//    {
//        #region Вложенные типы

//        /// <summary>Класс объектов сравнения сигнатур при десериализации</summary>
//        private sealed class SerializedSignatureComparer
//        {
//            /// <summary>Сигнатура</summary>
//            public string Signature { get; private set; }

//            private int Number { get; set; }

//            public SerializedSignatureComparer(string signature, int number)
//            {
//                if(signature == null)
//                    throw new ArgumentNullException(nameof(signature));
//                Signature = signature;
//                Number = number;
//            }

//            /// <summary>Сравнение сигнатур</summary>
//            /// <param name="a">Первая сравниниваемая сигнатура</param>
//            /// <param name="b">Вторая сравниниваемая сигнатура</param>
//            /// <returns>
//            /// 0  - если: номера совпадают, или если a и b == null
//            /// -1 - если:
//            ///   а == null и b != null, или номер a меньше номера b
//            /// +1 - если:
//            ///   a != null b b == null, или номер a больше номера b 
//            /// </returns>
//            public static int Compare(SerializedSignatureComparer a, SerializedSignatureComparer b)
//            {
//                return a == null 
//					? (ReferenceEquals(b, null) ? 0 : -1)
//                    : (b == null 
//						? 1
//						: (a.Number > b.Number 
//							? 1 
//							: (a.Number < b.Number ? -1 : 0)));
//            }
//        }

//        #endregion

//        /// <summary>Список операций</summary>
//        private readonly List<Operation> f_OperationsList = new List<Operation>();

//        /// <summary>Словарь приоритетов</summary>
//        private readonly Dictionary<int, PriorityAssociation> f_PriorityAssociationsDictionary
//            = new Dictionary<int, PriorityAssociation>();

//        /// <summary>Словарь операций</summary>
//        private readonly Dictionary<string, Operation> f_OperationNamesDictionary
//            = new Dictionary<string, Operation>();

//        /// <summary>Словарь сигнатур операций</summary>
//        private readonly Dictionary<string, ICollection<Operation>> f_OperationSignaturesDictionary
//            = new Dictionary<string, ICollection<Operation>>();

//        /// <summary>Определён ли приоритет</summary>
//        /// <param name="priority">Приоритет</param>
//        /// <returns>Истина, если указанный приоритет определён</returns>
//        public bool IsPriorityDefined(int priority)
//        {
//            return f_PriorityAssociationsDictionary.ContainsKey(priority);
//        }

//        public PriorityAssociation GetAssociationByPriority(int priority)
//        {
//            if(!IsPriorityDefined(priority))
//                throw new ArgumentException("Указанный приоритет не определён", nameof(priority));
//            return f_PriorityAssociationsDictionary[priority];
//        }

//        /// <summary>Проверка - определена ли операция?</summary>
//        /// <param name="OperationName">Имя операци</param>
//        /// <returns>Истина, если операция определена</returns>
//        public bool IsOperationDefined(string OperationName)
//        {
//            if(OperationName == null)
//                throw new ArgumentNullException("OperationName", "Указана null-ссылка в качестве имени операци");
//            if(OperationName.Length == 0)
//                throw new ArgumentException("Указано пустое имя операции", nameof(OperationName));
//            return f_OperationNamesDictionary.ContainsKey(OperationName);
//        }

//        /// <summary>Извлечение операции по имени</summary>
//        /// <param name="OperationName">Имя операции</param>
//        /// <returns>Операция с запрошенным именем</returns>
//        public Operation GetOperationByName(string OperationName)
//        {
//            if(!IsOperationDefined(OperationName))
//                throw new ArgumentException("Не определено операций по указанному имени", nameof(OperationName));
//            return f_OperationNamesDictionary[OperationName];
//        }

//        /// <summary>Проверка - определена ли сигнатура?</summary>
//        /// <param name="Signature">Сигнатуры</param>
//        /// <returns>Истина, если сигнатура определена</returns>
//        public bool IsSignatureDefined(string Signature)
//        {
//            if(Signature == null)
//                throw new ArgumentNullException(nameof(Signature));
//            if(Signature.Length == 0)
//                throw new ArgumentException("Указана пустая строка в качестве сигнатуры", nameof(Signature));
//            return f_OperationSignaturesDictionary.ContainsKey(Signature);
//        }

//        /// <summary>Получить операции по сигнатуре</summary>
//        /// <param name="Signature">Сигнатура</param>
//        /// <returns>Перечисление найденных сигнатур</returns>
//        public IEnumerable<Operation> GetOperationsUsingSignature(string Signature)
//        {
//            if(!IsSignatureDefined(Signature))
//                throw new ArgumentException("Сигнатура не определена", nameof(Signature));
//            return f_OperationSignaturesDictionary[Signature];
//        }

//        /// <summary>Длины сигнатур</summary>
//        public int[] SignaturesLens { get; private set; }

//        /// <summary>Загрузка конфигурации из встроенного ресурса xml</summary>
//        private void InitializeFromConfigurationXml()
//        {
//            const string lc_DocumentPath = "Operations.xml";
//            const string lc_SchemaPath = "Operations.xsd";

//            var assembly = Assembly.GetExecutingAssembly();
//            var lv_AssemblyName = assembly.GetName();

//            using(var stream = assembly.GetManifestResourceStream(
//                string.Format("{0}.{1}", lv_AssemblyName.Name, lc_DocumentPath)))
//            {
//                if(stream == null)
//                    throw new InvalidOperationException(
//                        string.Format("Не могу загрузить '{0}' конфигурационный ресурс из сборки", lc_DocumentPath));

//                var document = new XmlDocument();
//                document.Load(stream);

//                // Validating schema
//                XmlSchema schema;
//                using(var lv_StreamSchema = assembly.GetManifestResourceStream(
//                    string.Format("{0}.{1}", lv_AssemblyName.Name, lc_SchemaPath)))
//                {
//                    if(lv_StreamSchema == null)
//                        throw new InvalidOperationException("Не могу загрузить схему из сборки");
//                    schema = XmlSchema.Read(lv_StreamSchema, (s, e) =>
//                        {
//                            if(e.Exception != null)
//                                throw new InvalidOperationException("Неверная схема", e.Exception);
//                        });
//                }

//                document.Schemas.Add(schema);
//                document.Validate((s, e) =>
//                    {
//                        if(e.Exception != null)
//                            throw new InvalidOperationException("Неверный конфигурационный документ", e.Exception);
//                    });

//                var lv_NamespaceManager = new XmlNamespaceManager(document.NameTable);
//                lv_NamespaceManager.AddNamespace("main", schema.TargetNamespace);

//                var lv_PrioritiesNodeList =
//                    document.SelectNodes("main:operations/main:priorities/main:priority", lv_NamespaceManager);
//                if(lv_PrioritiesNodeList == null)
//                    throw new InvalidOperationException("Не могу загрузить приоритеты из конфигурации");

//                foreach(XmlNode lv_PriorityNode in lv_PrioritiesNodeList)
//                {
//                    Debug.Assert(lv_PriorityNode.Attributes != null, "priorityNode.Attributes != null");
//                    var lv_PriorityValue = Convert.ToInt32(lv_PriorityNode.Attributes["value"].Value);
//                    var lv_PriorityAssociation = (lv_PriorityNode.Attributes["association"].Value == "left") ?
//                        PriorityAssociation.LeftAssociated : PriorityAssociation.RightAssociated;
//                    f_PriorityAssociationsDictionary.Add(lv_PriorityValue, lv_PriorityAssociation);
//                }

//                var lv_OperationsNodeList
//                    = document.SelectNodes("main:operations/main:operations/main:operation", lv_NamespaceManager);
//                if(lv_OperationsNodeList == null)
//                    throw new InvalidOperationException("Не могу загрузить операторы из конфигурации");

//                foreach(XmlNode lv_OperationNode in lv_OperationsNodeList)
//                {
//                    Debug.Assert(lv_OperationNode.Attributes != null, "lv_OperationNode.Attributes != null");
//                    var lv_OperationName = lv_OperationNode.Attributes["name"].Value;
//                    var lv_OperationOperandsCount = Convert.ToInt32(lv_OperationNode.Attributes["operands"].Value);
//                    var lv_OperationKind = (lv_OperationNode.Attributes["kind"].Value == "operator") ?
//                        OperationKind.Operator : OperationKind.Function;
//                    var lv_OperationPriority = lv_OperationKind == OperationKind.Operator
//                        ? Convert.ToInt32(lv_OperationNode.Attributes["priority"].Value)
//                        : 0;

//                    var lv_SignaturesOrderedList = new List<SerializedSignatureComparer>();
//                    XmlNode lv_SignaturesNode = lv_OperationNode["signatures"];
//                    if(lv_SignaturesNode == null)
//                        throw new InvalidOperationException("Не найдено сигнатур операций");
//                    foreach(XmlNode lv_SignatureNode in lv_SignaturesNode.ChildNodes)
//                    {
//                        Debug.Assert(lv_SignatureNode.Attributes != null, "lv_SignatureNode.Attributes != null");
//                        lv_SignaturesOrderedList.Add(new SerializedSignatureComparer(
//                            lv_SignatureNode.Attributes["value"].Value,
//                            int.Parse(lv_SignatureNode.Attributes["number"].Value)));
//                    }
//                    lv_SignaturesOrderedList.Sort(SerializedSignatureComparer.Compare);
//                    var lv_OperationSignatures = new string[lv_SignaturesOrderedList.Count];
//                    for(var i = 0; i < lv_SignaturesOrderedList.Count; i++)
//                        lv_OperationSignatures[i] = lv_SignaturesOrderedList[i].Signature;

//                    XmlNode lv_CalculatorNode = lv_OperationNode["calculator"];
//                    if(lv_CalculatorNode == null)
//                        throw new InvalidOperationException("Не найдено вычислителей");
//                    Debug.Assert(lv_CalculatorNode.Attributes != null, "lv_CalculatorNode.Attributes != null");
//                    var lv_CalculatorType = lv_CalculatorNode.Attributes["type"].Value;
//                    if(string.IsNullOrEmpty(lv_CalculatorType))
//                        throw new InvalidOperationException("Пустая строка типа вычислителя");
//                    var lv_CalculatorTypeParts = lv_CalculatorType.Split(',');
//                    if(lv_CalculatorTypeParts.Length != 2)
//                        throw new InvalidOperationException("Неверный синтаксис определения");
//                    var lv_CalculatorTypeName = lv_CalculatorTypeParts[0];
//                    var lv_CalculatorAssemblyName = lv_CalculatorTypeParts[1];

//                    var lv_CalculatorAssembly = Assembly.Load(lv_CalculatorAssemblyName);
//                    var lv_OperationCalculator = (IOperationCalculator)lv_CalculatorAssembly
//                        .CreateInstance(lv_CalculatorTypeName, false);

//                    if(lv_OperationKind == OperationKind.Operator)
//                        f_OperationsList.Add(new Operation(lv_OperationName,
//                                                           OperationKind.Operator,
//                                                           lv_OperationSignatures,
//                                                           lv_OperationOperandsCount,
//                                                           lv_OperationCalculator,
//                                                           lv_OperationPriority));
//                    else
//                        f_OperationsList.Add(new Operation(lv_OperationName,
//                                                           OperationKind.Function,
//                                                           lv_OperationSignatures,
//                                                           lv_OperationOperandsCount,
//                                                           lv_OperationCalculator));
//                }
//            }
//        }

//        public OperationsRegistry()
//        {
//            InitializeFromConfigurationXml();
//            // Сохранение длин сигнатур, встреченных в процессе обработки
//            var lv_SignatureLengthList = new List<int>();
//            foreach(var operation in f_OperationsList)
//            {
//                f_OperationNamesDictionary.Add(operation.Name, operation);

//                // ЗАполнение словаря списками операций по соответствющим им сигнатурам
//                foreach(var signature in operation.Signature)
//                {
//                    if(!f_OperationSignaturesDictionary.ContainsKey(signature))
//                        f_OperationSignaturesDictionary.Add(signature, new List<Operation>());
//                    f_OperationSignaturesDictionary[signature].Add(operation);
//                }

//                // Добавление длины сигнатуры, если она встречена впервые
//                foreach(var lv_SignatureLength in
//                            operation.Signature
//                                        .Select(signature => signature.Length)
//                                        .Where(SignatureLength =>
//                                               lv_SignatureLengthList.All(length => length != SignatureLength)))
//                    lv_SignatureLengthList.Add(lv_SignatureLength);
//            }
//            lv_SignatureLengthList.Sort();
//            //            SignaturesLens = new int[lv_SignatureLengthList.Count];
//            //            lv_SignatureLengthList.CopyTo(SignaturesLens);
//            SignaturesLens = lv_SignatureLengthList.ToArray();
//        }
//    }
//}
