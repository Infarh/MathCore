# Трекинг-файл: XML-комментарии публичного API

> Список файлов исходного кода проекта **MathCore**, требующих **добавления** или **доработки** XML-комментариев публичного API.
>
> Данные получены автоматически из предупреждений компилятора **CS1591** (отсутствие XML-комментария) при сборке `MathCore/MathCore.csproj` в конфигурации `Release`, framework `net10.0`.

## Общая статистика

- **Всего предупреждений CS1591:** 6290
- **Файлов с недостающими комментариями:** 382
- **Документировано в последнем коммите:** 28 файлов (строки 1–27, 29)
- **Методика:** сборка с генерацией XML-документации; предупреждение CS1591 выдаётся на каждый публичный тип/член без XML-комментария

## Условные обозначения статуса

- **⬜ Нужно добавить** — публичное API в файле не документировано
- **🟡 Нужно доработать** — комментарии неполные или отсутствуют для части членов
- **✅ Готово** — комментарии добавлены

## Перечень файлов (по убыванию числа недостающих комментариев)

| # | Файл | Кол-во CS1591 | Статус |
|---|------|:---:|------|
| 1 | MathCore/Expressions/Complex/ComplexExpression.cs | 182 | ✅ Готово |
| 2 | MathCore/Interval.cs | 142 | ✅ Готово |
| 3 | MathCore/Extensions/AsyncAwait/TaskEx.cs | 138 | ✅ Готово |
| 4 | MathCore/Statistic/RandomNumbers/PolyformRandomGenerator.cs | 106 | ✅ Готово |
| 5 | MathCore/Extensions/Numerics/DoubleArrayExtensions.cs | 96 | ✅ Готово |
| 6 | MathCore/Extensions/Numerics/DoubleExtensions.cs | 92 | ✅ Готово |
| 7 | MathCore/PE/Headers/NT.ImageOptionalHeader.cs | 72 | ✅ Готово |
| 8 | MathCore/PE/Headers/DOS.cs | 66 | ✅ Готово |
| 9 | MathCore/Graphs/TreeListNode.cs | 66 | ✅ Готово |
| 10 | MathCore/ValuedInterval.cs | 62 | ✅ Готово |
| 11 | MathCore/Matrix.float.cs | 58 | ✅ Готово |
| 12 | MathCore/Extensions/Linq/Linq2XmlExtensions.cs | 58 | ✅ Готово |
| 13 | MathCore/Matrix.decimal.cs | 58 | ✅ Готово |
| 14 | MathCore/Matrix.complex.cs | 58 | ✅ Готово |
| 15 | MathCore/Matrix.int.cs | 56 | ✅ Готово |
| 16 | MathCore/Extensions/AsyncAwait/PerformActionAwaitable.cs | 56 | ✅ Готово |
| 17 | MathCore/Extensions/Expressions/MathExpression.cs | 54 | ✅ Готово |
| 18 | MathCore/Extensions/String/StringExtensions.cs | 52 | ✅ Готово |
| 19 | MathCore/Extensions/Numerics/DecimalExtensions.cs | 50 | ✅ Готово |
| 20 | MathCore/ProgressMonitor.cs | 50 | ✅ Готово |
| 21 | MathCore/Extensions/AsyncAwait/IDisposableAsyncExtensions.cs | 50 | ✅ Готово |
| 22 | MathCore/Matrix.long.cs | 50 | ✅ Готово |
| 23 | MathCore/Expressions/ExpressionVisitorEx.cs | 48 | ✅ Готово |
| 24 | MathCore/IoC/ServiceManager.cs | 48 | ✅ Готово |
| 25 | MathCore/Statistic/Histogram.cs | 46 | ✅ Готово |
| 26 | MathCore/Logging/Log.cs | 46 | ✅ Готово |
| 27 | MathCore/EventHandlerRef.cs | 44 | ✅ Готово |
| 28 | MathCore/Expressions/ExpressionRebuilder.cs | 44 | ⬜ Нужно добавить |
| 29 | MathCore/Functions/Differentiable/Function.cs | 44 | ✅ Готово |
| 30 | MathCore/Extensions/Delegates/DelegateExtensions.cs | 42 | ⬜ Нужно добавить |
| 31 | MathCore/Geolocation/GPS.cs | 42 | ⬜ Нужно добавить |
| 32 | MathCore/Vectors/VectorND_double.cs | 42 | ⬜ Нужно добавить |
| 33 | MathCore/PrecessionTimer.cs | 42 | ⬜ Нужно добавить |
| 34 | MathCore/ObservableLinkedList.cs | 42 | ⬜ Нужно добавить |
| 35 | MathCore/IoC/ServiceRegistrations/ServiceRegistration.cs | 42 | ⬜ Нужно добавить |
| 36 | MathCore/Expressions/ExpressionMatrix.cs | 42 | ⬜ Нужно добавить |
| 37 | MathCore/Functions/Differentiable/ComplicatedFunctions.cs | 42 | ⬜ Нужно добавить |
| 38 | MathCore/Logging/LogItem.cs | 40 | ⬜ Нужно добавить |
| 39 | MathCore/TimeInterval.cs | 38 | ⬜ Нужно добавить |
| 40 | MathCore/HashBuilder.cs | 38 | ⬜ Нужно добавить |
| 41 | MathCore/Functions/Differentiable/OperatorBinary.cs | 38 | ⬜ Нужно добавить |
| 42 | MathCore/StateMashine.cs | 38 | ⬜ Нужно добавить |
| 43 | MathCore/Extensions/ReaderWriterLockSlimExtensions.cs | 38 | ⬜ Нужно добавить |
| 44 | MathCore/Extensions/Numerics/IntExtensions.cs | 38 | ⬜ Нужно добавить |
| 45 | MathCore/ConsoleChart.cs | 38 | ⬜ Нужно добавить |
| 46 | MathCore/Net/Http/Html/HElement.cs | 36 | ⬜ Нужно добавить |
| 47 | MathCore/ConcurrentList.cs | 36 | ⬜ Нужно добавить |
| 48 | MathCore/Extensions/IO/DirectoryInfoExtensions.cs | 36 | ⬜ Нужно добавить |
| 49 | MathCore/Expressions/Field.cs | 36 | ⬜ Нужно добавить |
| 50 | MathCore/Extensions/AsyncAwait/SynchronizationContextAwaitable.cs | 36 | ⬜ Нужно добавить |
| 51 | MathCore/DifferentialEquations/Numerical/RungeKuttaVector2.cs | 34 | ⬜ Нужно добавить |
| 52 | MathCore/Values/RollingMaxRef.cs | 34 | ⬜ Нужно добавить |
| 53 | MathCore/Extensions/AsyncAwait/TaskSchedulerAwaitable.cs | 34 | ⬜ Нужно добавить |
| 54 | MathCore/DifferentialEquations/Numerical/RungeKuttaVector3.cs | 34 | ⬜ Нужно добавить |
| 55 | MathCore/DifferentialEquations/Numerical/RungeKuttaMatrix.cs | 34 | ⬜ Нужно добавить |
| 56 | MathCore/Extensions/ObjectReflectionPropertiesExtensions.cs | 34 | ⬜ Нужно добавить |
| 57 | MathCore/Extensions/IO/BinaryReaderExtensions.cs | 32 | ⬜ Нужно добавить |
| 58 | MathCore/IoC/ServiceManager.Register.cs | 32 | ⬜ Нужно добавить |
| 59 | MathCore/PE/Headers/SectionHeader.cs | 32 | ⬜ Нужно добавить |
| 60 | MathCore/MatrixN.cs | 32 | ⬜ Нужно добавить |
| 61 | MathCore/DifferentialEquations/Numerical/RungeKuttaComplex.cs | 32 | ⬜ Нужно добавить |
| 62 | MathCore/ReactiveLINQ/ObservableExtensions.cs | 30 | ⬜ Нужно добавить |
| 63 | MathCore/Values/Goertzel.cs | 30 | ⬜ Нужно добавить |
| 64 | MathCore/DifferentialEquations/Numerical/RungeKutta.cs | 30 | ⬜ Нужно добавить |
| 65 | MathCore/Vectors/SpaceAngle.cs | 30 | ⬜ Нужно добавить |
| 66 | MathCore/TransformationMatrix.cs | 28 | ⬜ Нужно добавить |
| 67 | MathCore/Values/PositionedString.cs | 28 | ⬜ Нужно добавить |
| 68 | MathCore/Extensions/AsyncAwait/YieldAwaitableThreadPool.cs | 28 | ⬜ Нужно добавить |
| 69 | MathCore/Extensions/IO/FileInfoExtensions.cs | 28 | ⬜ Нужно добавить |
| 70 | MathCore/RationalFunction.cs | 28 | ⬜ Нужно добавить |
| 71 | MathCore/Graphs/LambdaGraphNode.cs | 28 | ⬜ Нужно добавить |
| 72 | MathCore/Interpolation/Mapping.cs | 26 | ⬜ Нужно добавить |
| 73 | MathCore/SynchronizedQueue.cs | 26 | ⬜ Нужно добавить |
| 74 | MathCore/Expressions/Property.cs | 26 | ⬜ Нужно добавить |
| 75 | MathCore/Hash/CRC/CRC16.cs | 26 | ⬜ Нужно добавить |
| 76 | MathCore/Functions/Differentiable/Functions.cs | 26 | ⬜ Нужно добавить |
| 77 | MathCore/RefArrayView.cs | 26 | ⬜ Нужно добавить |
| 78 | MathCore/Extensions/IO/StreamReaderExtensions.cs | 26 | ⬜ Нужно добавить |
| 79 | MathCore/StringByteStream.cs | 24 | ⬜ Нужно добавить |
| 80 | MathCore/Hash/CRC/CRC32.cs | 24 | ⬜ Нужно добавить |
| 81 | MathCore/Extensions/INotifyPropertyChangedExtensions.cs | 24 | ⬜ Нужно добавить |
| 82 | MathCore/Statistic/RandomNumbers/RandomGenerator.cs | 24 | ⬜ Нужно добавить |
| 83 | MathCore/Interpolation/Lagrange.cs | 24 | ⬜ Нужно добавить |
| 84 | MathCore/Expressions/DifferentialVisitor.cs | 24 | ⬜ Нужно добавить |
| 85 | MathCore/Expressions/LambdaExpressionRebuilder.cs | 24 | ⬜ Нужно добавить |
| 86 | MathCore/MathParser/ExpressionTrees/Nodes/ComputedNode.cs | 24 | ⬜ Нужно добавить |
| 87 | MathCore/Polynom.Array.cs | 24 | ⬜ Нужно добавить |
| 88 | MathCore/Values/DifferentialWithAveraging.cs | 24 | ⬜ Нужно добавить |
| 89 | MathCore/Numeric.cs | 22 | ⬜ Нужно добавить |
| 90 | MathCore/IO/Base64Decoder.cs | 22 | ⬜ Нужно добавить |
| 91 | MathCore/ConsoleProgressBar.cs | 22 | ⬜ Нужно добавить |
| 92 | MathCore/IoC/ServiceRegistrations/SingletonServiceRegistration.cs | 22 | ⬜ Нужно добавить |
| 93 | MathCore/IoC/ServiceRegistrations/SingleThreadServiceRegistration.cs | 22 | ⬜ Нужно добавить |
| 94 | MathCore/Values/ByteCountValue.cs | 22 | ⬜ Нужно добавить |
| 95 | MathCore/Geolocation/GeoLocation.cs | 22 | ⬜ Нужно добавить |
| 96 | MathCore/Expressions/Visitors/ExpressionToXml.cs | 22 | ⬜ Нужно добавить |
| 97 | MathCore/Extensions/Json/IJsonTypeInfoResolverEx.cs | 22 | ⬜ Нужно добавить |
| 98 | MathCore/Xml/XPath/XPathQuery.cs | 20 | ⬜ Нужно добавить |
| 99 | MathCore/Values/TimeAverage2Value.cs | 20 | ⬜ Нужно добавить |
| 100 | MathCore/Functions/Differentiable/OperatorUnary.cs | 20 | ⬜ Нужно добавить |
| 101 | MathCore/Values/NumberedValues.cs | 20 | ⬜ Нужно добавить |
| 102 | MathCore/Extensions/ArrayExtensions.cs | 20 | ⬜ Нужно добавить |
| 103 | MathCore/Expressions/TeXExpressionVisitor.cs | 20 | ⬜ Нужно добавить |
| 104 | MathCore/Extensions/Json/JsonSerializerOptionsEx.cs | 20 | ⬜ Нужно добавить |
| 105 | MathCore/Expressions/ItemBase.cs | 20 | ⬜ Нужно добавить |
| 106 | MathCore/Expressions/ObjectDescriptor.cs | 20 | ⬜ Нужно добавить |
| 107 | MathCore/Extensions/Reflection/AssemblyEx.cs | 20 | ⬜ Нужно добавить |
| 108 | MathCore/Collections/FTree.cs | 18 | ⬜ Нужно добавить |
| 109 | MathCore/Xml/DXmlNode.cs | 18 | ⬜ Нужно добавить |
| 110 | MathCore/DataGenericSources/DataHost.cs | 18 | ⬜ Нужно добавить |
| 111 | MathCore/Graphs/GraphRoute.cs | 18 | ⬜ Нужно добавить |
| 112 | MathCore/Extensions/Numerics/CharExtensions.cs | 18 | ⬜ Нужно добавить |
| 113 | MathCore/DifferentialEquations/Numerical/Euler.cs | 18 | ⬜ Нужно добавить |
| 114 | MathCore/Extensions/PointsEx.cs | 18 | ⬜ Нужно добавить |
| 115 | MathCore/Interpolation/Interpolator.cs | 18 | ⬜ Нужно добавить |
| 116 | MathCore/Extensions/Numerics/ShortExtensions.cs | 18 | ⬜ Нужно добавить |
| 117 | MathCore/Extensions/Numerics/ByteExtensions.cs | 18 | ⬜ Нужно добавить |
| 118 | MathCore/DifferentialEquations/Numerical/EulerModified.cs | 18 | ⬜ Нужно добавить |
| 119 | MathCore/PropertyEqualityComparer.cs | 18 | ⬜ Нужно добавить |
| 120 | MathCore/IO/Base64Encoder.cs | 16 | ⬜ Нужно добавить |
| 121 | MathCore/IoC/Exceptions/ServiceConstructorNotFoundException.cs | 16 | ⬜ Нужно добавить |
| 122 | MathCore/Randomizer.cs | 16 | ⬜ Нужно добавить |
| 123 | MathCore/Geolocation/GeoLocationSpan.cs | 16 | ⬜ Нужно добавить |
| 124 | MathCore/IoC/ServiceRegistrations/ServiceRegistration.ServiceConstructorInfo.cs | 16 | ⬜ Нужно добавить |
| 125 | MathCore/MatrixT.Array.Operator.cs | 16 | ⬜ Нужно добавить |
| 126 | MathCore/Monads/Maybe.cs | 16 | ⬜ Нужно добавить |
| 127 | MathCore/Expressions/ExpressionEx.cs | 16 | ⬜ Нужно добавить |
| 128 | MathCore/Vectors/Vector.cs | 16 | ⬜ Нужно добавить |
| 129 | MathCore/Extensions/IEnumerableExtensions.cs | 16 | ⬜ Нужно добавить |
| 130 | MathCore/DataGenericSources/DataSource.cs | 16 | ⬜ Нужно добавить |
| 131 | MathCore/IO/TextFileContentMonitor.cs | 14 | ⬜ Нужно добавить |
| 132 | MathCore/PropertyComparer.cs | 14 | ⬜ Нужно добавить |
| 133 | MathCore/Interpolation/Newton.cs | 14 | ⬜ Нужно добавить |
| 134 | MathCore/Extensions/AsyncAwait/YieldAwaitableThread.cs | 14 | ⬜ Нужно добавить |
| 135 | MathCore/Values/AverageValue.cs | 14 | ⬜ Нужно добавить |
| 136 | MathCore/PE/Headers/NT.cs | 14 | ⬜ Нужно добавить |
| 137 | MathCore/IniFile.cs | 14 | ⬜ Нужно добавить |
| 138 | MathCore/DifferentialEquations/Numerical/AdamsMoulton.cs | 14 | ⬜ Нужно добавить |
| 139 | MathCore/DifferentialEquations/Numerical/AdamsBashforth.cs | 14 | ⬜ Нужно добавить |
| 140 | MathCore/Hash/CRC/CRC8.cs | 14 | ⬜ Нужно добавить |
| 141 | MathCore/Extensions/IO/BinaryWriterExtensions.cs | 14 | ⬜ Нужно добавить |
| 142 | MathCore/Statistic/RandomNumbers/TriangularRandomGenerator.cs | 14 | ⬜ Нужно добавить |
| 143 | MathCore/MatrixArray.cs | 14 | ⬜ Нужно добавить |
| 144 | MathCore/CSV/CSVQuery.cs | 14 | ⬜ Нужно добавить |
| 145 | MathCore/Extensions/Numerics/LongExtensions.cs | 14 | ⬜ Нужно добавить |
| 146 | MathCore/Monads/WorkFlow/WorkResult.cs | 14 | ⬜ Нужно добавить |
| 147 | MathCore/IoC/IServiceManager.RegisterSingleton.cs | 12 | ⬜ Нужно добавить |
| 148 | MathCore/Interpolation/Linear.cs | 12 | ⬜ Нужно добавить |
| 149 | MathCore/Values/AverageExpValue.cs | 12 | ⬜ Нужно добавить |
| 150 | MathCore/Functions/Service.cs | 12 | ⬜ Нужно добавить |
| 151 | MathCore/Statistic/RandomNumbers/NormalRandomGenerator.cs | 12 | ⬜ Нужно добавить |
| 152 | MathCore/ReactiveLINQ/TimeIntervalObservable.cs | 12 | ⬜ Нужно добавить |
| 153 | MathCore/RandomRef.cs | 12 | ⬜ Нужно добавить |
| 154 | MathCore/Values/LinearQueue.cs | 12 | ⬜ Нужно добавить |
| 155 | MathCore/ReactiveLINQ/LambdaObserver.cs | 12 | ⬜ Нужно добавить |
| 156 | MathCore/Trees/TreeNode.cs | 12 | ⬜ Нужно добавить |
| 157 | MathCore/IoC/ServiceManager.RegisterSingleton.cs | 12 | ⬜ Нужно добавить |
| 158 | MathCore/Extensions/IO/FileSystemWatcherExtensions.cs | 12 | ⬜ Нужно добавить |
| 159 | MathCore/Threading/Tasks/SimpleProgress.cs | 12 | ⬜ Нужно добавить |
| 160 | MathCore/Threading/Tasks/Schedulers/SynchronizationContextScheduler.cs | 12 | ⬜ Нужно добавить |
| 161 | MathCore/Values/MinMaxValue.cs | 12 | ⬜ Нужно добавить |
| 162 | MathCore/Extensions/StringBuilderExtensions.cs | 12 | ⬜ Нужно добавить |
| 163 | MathCore/Net/Http/Html/HElementBase.cs | 12 | ⬜ Нужно добавить |
| 164 | MathCore/IoC/ServiceRegistrations/SingleCallServiceRegistration.cs | 10 | ⬜ Нужно добавить |
| 165 | MathCore/IoC/ServiceManager.RegisterSingleTask.cs | 10 | ⬜ Нужно добавить |
| 166 | MathCore/IoC/ServiceManager.RegisterSingleCall.cs | 10 | ⬜ Нужно добавить |
| 167 | MathCore/ViewModels/ViewModel.PropertyChangedEventsSupressor.cs | 10 | ⬜ Нужно добавить |
| 168 | MathCore/Vectors/Vector2D.Extensions.cs | 10 | ⬜ Нужно добавить |
| 169 | MathCore/PE/Headers/NT.ImageFileHeader.cs | 10 | ⬜ Нужно добавить |
| 170 | MathCore/Net/Http/Html/NumberedList.cs | 10 | ⬜ Нужно добавить |
| 171 | MathCore/LambdaToStringObjectIndicator.cs | 10 | ⬜ Нужно добавить |
| 172 | MathCore/Vectors/VectorNd.cs | 10 | ⬜ Нужно добавить |
| 173 | MathCore/Net/Http/Html/HAttribute.cs | 10 | ⬜ Нужно добавить |
| 174 | MathCore/DifferentialEquations/Numerical/EquationSystemMethods.cs | 10 | ⬜ Нужно добавить |
| 175 | MathCore/DifferentialEquations/Numerical/Differential.cs | 10 | ⬜ Нужно добавить |
| 176 | MathCore/Logging/LogType.cs | 10 | ⬜ Нужно добавить |
| 177 | MathCore/PE/Headers/Header.cs | 10 | ⬜ Нужно добавить |
| 178 | MathCore/Vectors/Vector3D.Extensions.cs | 10 | ⬜ Нужно добавить |
| 179 | MathCore/Data/Validate.cs | 10 | ⬜ Нужно добавить |
| 180 | MathCore/Net/Http/Html/Link.cs | 10 | ⬜ Нужно добавить |
| 181 | MathCore/Net/Http/Html/MarkedList.cs | 10 | ⬜ Нужно добавить |
| 182 | MathCore/Net/Http/Html/Text.cs | 10 | ⬜ Нужно добавить |
| 183 | MathCore/Net/Http/Html/Script.cs | 10 | ⬜ Нужно добавить |
| 184 | MathCore/Net/Http/Html/Page.cs | 10 | ⬜ Нужно добавить |
| 185 | MathCore/Complex.Net8.cs | 10 | ⬜ Нужно добавить |
| 186 | MathCore/MathParser/DifferentialTransformationVisitor.cs | 10 | ⬜ Нужно добавить |
| 187 | MathCore/IoC/IServiceManager.RegisterSingleThread.cs | 10 | ⬜ Нужно добавить |
| 188 | MathCore/IoC/IServiceManager.RegisterSingleTask.cs | 10 | ⬜ Нужно добавить |
| 189 | MathCore/Statistic/RandomNumbers/UniformRandomGenerator.cs | 10 | ⬜ Нужно добавить |
| 190 | MathCore/Statistic/Distributions.Erlang.cs | 10 | ⬜ Нужно добавить |
| 191 | MathCore/Fraction.cs | 10 | ⬜ Нужно добавить |
| 192 | MathCore/Extensions/RangeExtensions.cs | 10 | ⬜ Нужно добавить |
| 193 | MathCore/Extensions/RandomExtensions.cs | 10 | ⬜ Нужно добавить |
| 194 | MathCore/StreamWrapper.cs | 10 | ⬜ Нужно добавить |
| 195 | MathCore/Functions/PSO/PSOFuncExtensions.cs | 10 | ⬜ Нужно добавить |
| 196 | MathCore/Functions/PSO/Swarm1D.cs | 10 | ⬜ Нужно добавить |
| 197 | MathCore/Hash/SHA512.cs | 10 | ⬜ Нужно добавить |
| 198 | MathCore/Interpolation/CubicSpline.cs | 10 | ⬜ Нужно добавить |
| 199 | MathCore/Extensions/DateTimeExtensions.cs | 10 | ⬜ Нужно добавить |
| 200 | MathCore/Extensions/AsyncAwait/ProcessExtensions.cs | 10 | ⬜ Нужно добавить |
| 201 | MathCore/Hash/SHA256.cs | 10 | ⬜ Нужно добавить |
| 202 | MathCore/IoC/ServiceManager.RegisterSingleThread.cs | 10 | ⬜ Нужно добавить |
| 203 | MathCore/IoC/Exceptions/ServiceRegistrationException.cs | 10 | ⬜ Нужно добавить |
| 204 | MathCore/IoC/IServiceManager.RegisterSingleCall.cs | 10 | ⬜ Нужно добавить |
| 205 | MathCore/Expressions/PredicateBuilder.cs | 10 | ⬜ Нужно добавить |
| 206 | MathCore/Extensions/Linq/ToSql/ExpressionPropertyAttribute.cs | 8 | ⬜ Нужно добавить |
| 207 | MathCore/Functions/PSO/Swarm2D.cs | 8 | ⬜ Нужно добавить |
| 208 | MathCore/Net/Http/Html/Href.cs | 8 | ⬜ Нужно добавить |
| 209 | MathCore/Values/TimeAverageValue.cs | 8 | ⬜ Нужно добавить |
| 210 | MathCore/Expressions/Complex/ComplexLambdaBinaryExpression.cs | 8 | ⬜ Нужно добавить |
| 211 | MathCore/IoC/Exceptions/ServiceRegistrationNotFoundException.cs | 8 | ⬜ Нужно добавить |
| 212 | MathCore/Extensions/Linq/LinqEx.cs | 8 | ⬜ Нужно добавить |
| 213 | MathCore/Expressions/Complex/ComplexPowerExpression.cs | 8 | ⬜ Нужно добавить |
| 214 | MathCore/Expressions/MathExpressionSimplifier.cs | 8 | ⬜ Нужно добавить |
| 215 | MathCore/Complex.Extentions.cs | 8 | ⬜ Нужно добавить |
| 216 | MathCore/Net/Http/Html/DataListItem.cs | 8 | ⬜ Нужно добавить |
| 217 | MathCore/AccuracyEqualityComparer.cs | 8 | ⬜ Нужно добавить |
| 218 | MathCore/Expressions/Complex/ComplexLambdaUnaryExpression.cs | 8 | ⬜ Нужно добавить |
| 219 | MathCore/Threading/Tasks/ProgressSplitter.cs | 8 | ⬜ Нужно добавить |
| 220 | MathCore/Net/Http/Html/Table.cs | 8 | ⬜ Нужно добавить |
| 221 | MathCore/Extensions/DateTimeOffsetExtensions.cs | 8 | ⬜ Нужно добавить |
| 222 | MathCore/Exceptions/CalculationException.cs | 8 | ⬜ Нужно добавить |
| 223 | MathCore/LambdaDisposableObject.cs | 8 | ⬜ Нужно добавить |
| 224 | MathCore/LambdaEqualityComparer.cs | 8 | ⬜ Нужно добавить |
| 225 | MathCore/Extensions/AsyncAwait/LinqAsyncEx.cs | 8 | ⬜ Нужно добавить |
| 226 | MathCore/Values/LambdaSetOf.cs | 8 | ⬜ Нужно добавить |
| 227 | MathCore/Expressions/Complex/ComplexBinaryExpression.cs | 8 | ⬜ Нужно добавить |
| 228 | MathCore/Interpolation/Biliniar.cs | 8 | ⬜ Нужно добавить |
| 229 | MathCore/SequentialEnumerable.cs | 8 | ⬜ Нужно добавить |
| 230 | MathCore/Exceptions/ConnectionException.cs | 8 | ⬜ Нужно добавить |
| 231 | MathCore/IoC/MapServiceRegistration.cs | 8 | ⬜ Нужно добавить |
| 232 | MathCore/Hash/MD5.cs | 8 | ⬜ Нужно добавить |
| 233 | MathCore/Hash/HashAlgorithm.cs | 8 | ⬜ Нужно добавить |
| 234 | MathCore/Vectors/Vector3D.cs | 8 | ⬜ Нужно добавить |
| 235 | MathCore/Graphs/LambdaGraphLink.cs | 8 | ⬜ Нужно добавить |
| 236 | MathCore/SpecialFunctions.cs | 6 | ⬜ Нужно добавить |
| 237 | MathCore/ReactiveLINQ/SimpleObservableEx.cs | 6 | ⬜ Нужно добавить |
| 238 | MathCore/Values/TimeBufferedValue.cs | 6 | ⬜ Нужно добавить |
| 239 | MathCore/Net/Http/Html/TypedElement.cs | 6 | ⬜ Нужно добавить |
| 240 | MathCore/Threading/PauseTokenSource.cs | 6 | ⬜ Нужно добавить |
| 241 | MathCore/Vectors/Vector2D.cs | 6 | ⬜ Нужно добавить |
| 242 | MathCore/AccuracyComparer.cs | 6 | ⬜ Нужно добавить |
| 243 | MathCore/Hash/CRC/CRC64.cs | 6 | ⬜ Нужно добавить |
| 244 | MathCore/IoC/ServiceRegistrations/ViewModelAttribute.cs | 6 | ⬜ Нужно добавить |
| 245 | MathCore/Interpolation/MNK.cs | 6 | ⬜ Нужно добавить |
| 246 | MathCore/Hash/Streebog.cs | 6 | ⬜ Нужно добавить |
| 247 | MathCore/Extensions/SizeEx.cs | 6 | ⬜ Нужно добавить |
| 248 | MathCore/Extensions/QueueExtensions.cs | 6 | ⬜ Нужно добавить |
| 249 | MathCore/Extensions/ObjectExtentions.cs | 6 | ⬜ Нужно добавить |
| 250 | MathCore/Expressions/TeXEvaluationExpressionVisitor.cs | 6 | ⬜ Нужно добавить |
| 251 | MathCore/Matrix.ConvertOperator.cs | 6 | ⬜ Нужно добавить |
| 252 | MathCore/Expressions/SubstitutionVisitor.cs | 6 | ⬜ Нужно добавить |
| 253 | MathCore/Expressions/Method.cs | 6 | ⬜ Нужно добавить |
| 254 | MathCore/Expressions/Complex/ComplexUnaryExpression.cs | 6 | ⬜ Нужно добавить |
| 255 | MathCore/Extensions/IO/TextWriterExtensions.cs | 6 | ⬜ Нужно добавить |
| 256 | MathCore/Expressions/Complex/ComplexConstantExpression.cs | 6 | ⬜ Нужно добавить |
| 257 | MathCore/MatrixT.Algorithms.cs | 6 | ⬜ Нужно добавить |
| 258 | MathCore/DictionaryKeySafe.cs | 6 | ⬜ Нужно добавить |
| 259 | MathCore/Data/PropertyLink.cs | 6 | ⬜ Нужно добавить |
| 260 | MathCore/Statistic/Distributions.cs | 4 | ⬜ Нужно добавить |
| 261 | MathCore/SpecialFunctions.IncompliteBeta.cs | 4 | ⬜ Нужно добавить |
| 262 | MathCore/Functions/FunctionsExtensions.cs | 4 | ⬜ Нужно добавить |
| 263 | MathCore/Complex.Trigonomerty.cs | 4 | ⬜ Нужно добавить |
| 264 | MathCore/Interpolation/IInterpolator.cs | 4 | ⬜ Нужно добавить |
| 265 | MathCore/Complex.IBinaryFloatingPointIeee754.cs | 4 | ⬜ Нужно добавить |
| 266 | MathCore/RandomNormal.cs | 4 | ⬜ Нужно добавить |
| 267 | MathCore/Queries/QueryResult.cs | 4 | ⬜ Нужно добавить |
| 268 | MathCore/Complex.cs | 4 | ⬜ Нужно добавить |
| 269 | MathCore/ProgressTimeoutDecimator.cs | 4 | ⬜ Нужно добавить |
| 270 | MathCore/IProgressControl.cs | 4 | ⬜ Нужно добавить |
| 271 | MathCore/ProgressCallCountDecimator.cs | 4 | ⬜ Нужно добавить |
| 272 | MathCore/JSON/JSONObject.cs | 4 | ⬜ Нужно добавить |
| 273 | MathCore/IoC/IServiceManager.Register.cs | 4 | ⬜ Нужно добавить |
| 274 | MathCore/StreamingObjectReader.cs | 4 | ⬜ Нужно добавить |
| 275 | MathCore/Extensions/ObservableCollectionExtensions.cs | 4 | ⬜ Нужно добавить |
| 276 | MathCore/Expressions/Complex/ComplexInverseExpression.cs | 4 | ⬜ Нужно добавить |
| 277 | MathCore/Expressions/Complex/ComplexConjugateExpression.cs | 4 | ⬜ Нужно добавить |
| 278 | MathCore/ExpandableList.cs | 4 | ⬜ Нужно добавить |
| 279 | MathCore/ExceptionEventHandlerArgs.cs | 4 | ⬜ Нужно добавить |
| 280 | MathCore/Values/GoertzelVector.cs | 4 | ⬜ Нужно добавить |
| 281 | MathCore/Values/DictionaryReadOnly.cs | 4 | ⬜ Нужно добавить |
| 282 | MathCore/Extensions/AsyncAwait/YieldAsyncAwaitable.cs | 4 | ⬜ Нужно добавить |
| 283 | MathCore/Values/AverageAdaptiveValue.cs | 4 | ⬜ Нужно добавить |
| 284 | MathCore/Extensions/Delegates/FuncExtensions.cs | 4 | ⬜ Нужно добавить |
| 285 | MathCore/TypeConverter.cs | 4 | ⬜ Нужно добавить |
| 286 | MathCore/Extensions/IO/FileSystemInfoExtensions.cs | 4 | ⬜ Нужно добавить |
| 287 | MathCore/LambdaComparer.cs | 4 | ⬜ Нужно добавить |
| 288 | MathCore/Extensions/IO/ZipArchiveExtensions.cs | 4 | ⬜ Нужно добавить |
| 289 | MathCore/Extensions/Numerics/BigIntegerExtensions.cs | 4 | ⬜ Нужно добавить |
| 290 | MathCore/Threading/PauseToken.cs | 4 | ⬜ Нужно добавить |
| 291 | MathCore/Threading/InstanceThreadPool.cs | 4 | ⬜ Нужно добавить |
| 292 | MathCore/Vectors/Vector3D.Operators.cs | 4 | ⬜ Нужно добавить |
| 293 | MathCore/PE/Tables/IMAGE_IMPORT_DESCRIPTOR.cs | 4 | ⬜ Нужно добавить |
| 294 | MathCore/Net/Http/Html/Article.cs | 4 | ⬜ Нужно добавить |
| 295 | MathCore/Matrix.Algorithms.cs | 4 | ⬜ Нужно добавить |
| 296 | MathCore/Net/Http/Html/Header.cs | 4 | ⬜ Нужно добавить |
| 297 | MathCore/Net/Http/Html/TableHeader.cs | 4 | ⬜ Нужно добавить |
| 298 | MathCore/Net/Http/Html/Head.cs | 4 | ⬜ Нужно добавить |
| 299 | MathCore/Net/Http/Html/TableBody.cs | 4 | ⬜ Нужно добавить |
| 300 | MathCore/Net/Http/Html/Section.cs | 4 | ⬜ Нужно добавить |
| 301 | MathCore/Net/Http/Html/IdAttribute.cs | 4 | ⬜ Нужно добавить |
| 302 | MathCore/ObjectPool.cs | 4 | ⬜ Нужно добавить |
| 303 | MathCore/PE/Tables/IMAGE_EXPORT_DIRECTORY.cs | 4 | ⬜ Нужно добавить |
| 304 | MathCore/Net/Http/Html/MenuList.cs | 4 | ⬜ Нужно добавить |
| 305 | MathCore/Net/Http/Html/H4.cs | 4 | ⬜ Нужно добавить |
| 306 | MathCore/Net/Http/Html/H3.cs | 4 | ⬜ Нужно добавить |
| 307 | MathCore/Net/Http/Html/P.cs | 4 | ⬜ Нужно добавить |
| 308 | MathCore/Net/Http/Html/H2.cs | 4 | ⬜ Нужно добавить |
| 309 | MathCore/MathParser/ExpressionTrees/Nodes/IntervalNode.cs | 4 | ⬜ Нужно добавить |
| 310 | MathCore/Net/Http/Html/H.cs | 4 | ⬜ Нужно добавить |
| 311 | MathCore/Net/Http/Html/H1.cs | 4 | ⬜ Нужно добавить |
| 312 | MathCore/Net/Http/Html/Footer.cs | 4 | ⬜ Нужно добавить |
| 313 | MathCore/Net/Http/Html/Body.cs | 4 | ⬜ Нужно добавить |
| 314 | MathCore/PE/ImportedFunctions.cs | 4 | ⬜ Нужно добавить |
| 315 | MathCore/LambdaDictionary.cs | 4 | ⬜ Нужно добавить |
| 316 | MathCore/LambdaFormatter.cs | 4 | ⬜ Нужно добавить |
| 317 | MathCore/LambdaProcessor.cs | 4 | ⬜ Нужно добавить |
| 318 | MathCore/LambdaProperty.cs | 4 | ⬜ Нужно добавить |
| 319 | MathCore/PatternString.cs | 4 | ⬜ Нужно добавить |
| 320 | MathCore/Net/Http/Html/ListItem.cs | 4 | ⬜ Нужно добавить |
| 321 | MathCore/Net/Http/Html/ClassAttribute.cs | 4 | ⬜ Нужно добавить |
| 322 | MathCore/Net/Http/Html/DataList.cs | 4 | ⬜ Нужно добавить |
| 323 | MathCore/MathParser/DifferentialOperator.cs | 4 | ⬜ Нужно добавить |
| 324 | MathCore/Net/Http/Html/div.cs | 4 | ⬜ Нужно добавить |
| 325 | MathCore/MathEx.cs | 4 | ⬜ Нужно добавить |
| 326 | MathCore/Net/Http/Html/Title.cs | 4 | ⬜ Нужно добавить |
| 327 | MathCore/Values/CSV.cs | 2 | ⬜ Нужно добавить |
| 328 | MathCore/Extensions/AsyncAwait/YieldAwaiterExtensions.cs | 2 | ⬜ Нужно добавить |
| 329 | MathCore/Extensions/AsyncAwait/AsyncExtensions.cs | 2 | ⬜ Нужно добавить |
| 330 | MathCore/Matrix.Array.Operator.cs | 2 | ⬜ Нужно добавить |
| 331 | MathCore/Matrix.Array.cs | 2 | ⬜ Нужно добавить |
| 332 | MathCore/Expressions/Complex/ComplexDivideExpression.cs | 2 | ⬜ Нужно добавить |
| 333 | MathCore/Values/MaxValue.cs | 2 | ⬜ Нужно добавить |
| 334 | MathCore/Expressions/Complex/ComplexSubtractExpression.cs | 2 | ⬜ Нужно добавить |
| 335 | MathCore/Expressions/Complex/ComplexMultiplyExpression.cs | 2 | ⬜ Нужно добавить |
| 336 | MathCore/Matrix.double.cs | 2 | ⬜ Нужно добавить |
| 337 | MathCore/Values/StatisticValue.cs | 2 | ⬜ Нужно добавить |
| 338 | MathCore/Values/StreamDataSpeedValue.cs | 2 | ⬜ Нужно добавить |
| 339 | MathCore/Vectors/Fields/VectorField3D.cs | 2 | ⬜ Нужно добавить |
| 340 | MathCore/Expressions/Complex/ComplexAddExpression.cs | 2 | ⬜ Нужно добавить |
| 341 | MathCore/Expressions/CloningVisitor.cs | 2 | ⬜ Нужно добавить |
| 342 | MathCore/Complex.Operators.cs | 2 | ⬜ Нужно добавить |
| 343 | MathCore/ViewModels/ViewModel.Set.cs | 2 | ⬜ Нужно добавить |
| 344 | MathCore/ViewModels/ViewModel.SetValueResult.cs | 2 | ⬜ Нужно добавить |
| 345 | MathCore/Matrix.cs | 2 | ⬜ Нужно добавить |
| 346 | MathCore/PE/PEResources.cs | 2 | ⬜ Нужно добавить |
| 347 | MathCore/MathParser/ExpressionTrees/Nodes/SelectorOperatorNode.cs | 2 | ⬜ Нужно добавить |
| 348 | MathCore/MathParser/VariablesCollection.cs | 2 | ⬜ Нужно добавить |
| 349 | MathCore/SpecialFunctions.Distribution.cs | 2 | ⬜ Нужно добавить |
| 350 | MathCore/MathParser/ExpressionTrees/Nodes/DivisionOperatorNode.cs | 2 | ⬜ Нужно добавить |
| 351 | MathCore/SpecialFunctions.Bessel.cs | 2 | ⬜ Нужно добавить |
| 352 | MathCore/Graphs/IGraphNode.cs | 2 | ⬜ Нужно добавить |
| 353 | MathCore/SelectableCollection.cs | 2 | ⬜ Нужно добавить |
| 354 | MathCore/MatrixT.cs | 2 | ⬜ Нужно добавить |
| 355 | MathCore/ObservableHashSet.cs | 2 | ⬜ Нужно добавить |
| 356 | MathCore/Interpolation/BezierCurve.cs | 2 | ⬜ Нужно добавить |
| 357 | MathCore/ReactiveLINQ/LambdaObservable.cs | 2 | ⬜ Нужно добавить |
| 358 | MathCore/Interpolation/Spline2D.cs | 2 | ⬜ Нужно добавить |
| 359 | MathCore/IO/Win32Processes.cs | 2 | ⬜ Нужно добавить |
| 360 | MathCore/PE/Headers/IMAGE_DIRECTORY_ENTRY.cs | 2 | ⬜ Нужно добавить |
| 361 | MathCore/IoC/ServiceManagerAccessor.cs | 2 | ⬜ Нужно добавить |
| 362 | MathCore/PE/PEFile.cs | 2 | ⬜ Нужно добавить |
| 363 | MathCore/PE/ExportedFunctions.cs | 2 | ⬜ Нужно добавить |
| 364 | MathCore/Extensions/ByteArrayExtensions.cs | 2 | ⬜ Нужно добавить |
| 365 | MathCore/SpecialFunctions.TrigonometryIntegrals.cs | 2 | ⬜ Нужно добавить |
| 366 | MathCore/Extensions/Xml/XmlWriterExtensions.cs | 2 | ⬜ Нужно добавить |
| 367 | MathCore/Extensions/IDictionaryExtensions.cs | 2 | ⬜ Нужно добавить |
| 368 | MathCore/Extensions/INotifyCollectionChangedExtensions.cs | 2 | ⬜ Нужно добавить |
| 369 | MathCore/TimerAsync.cs | 2 | ⬜ Нужно добавить |
| 370 | MathCore/MathParser/ExpressionTrees/Nodes/VariableValueNode.cs | 2 | ⬜ Нужно добавить |
| 371 | MathCore/Extensions/IProgressExtensions.cs | 2 | ⬜ Нужно добавить |
| 372 | MathCore/MathParser/ExpressionTrees/Nodes/OperatorNode.cs | 2 | ⬜ Нужно добавить |
| 373 | MathCore/SpecialFunctionsErf.cs | 2 | ⬜ Нужно добавить |
| 374 | MathCore/MathParser/ExpressionTrees/Nodes/MultiplicationOperatorNode.cs | 2 | ⬜ Нужно добавить |
| 375 | MathCore/Extensions/String/StringExtentions.Parse.cs | 2 | ⬜ Нужно добавить |
| 376 | MathCore/Extensions/TimeSpanExtensions.cs | 2 | ⬜ Нужно добавить |
| 377 | MathCore/Extensions/TupleEx.cs | 2 | ⬜ Нужно добавить |
| 378 | MathCore/Extensions/Xml/IXmlSerializableExtensions.cs | 2 | ⬜ Нужно добавить |
| 379 | MathCore/Extensions/Xml/XDocumentExtensions.cs | 2 | ⬜ Нужно добавить |
| 380 | MathCore/Extensions/Xml/XmlReaderExtensions.cs | 2 | ⬜ Нужно добавить |
| 381 | MathCore/MathParser/ExpressionTrees/Nodes/ExpressionTreeNode.cs | 2 | ⬜ Нужно добавить |
| 382 | MathCore/Xml/XPath/XPathReaderException.cs | 2 | ⬜ Нужно добавить |

