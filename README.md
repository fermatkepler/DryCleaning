**Interpretación del enunciado**

He entendido que se pide una web API:

* que tiene estas APIs para marcar las horas de apertura y cierre. Pongo su nombre interno y después el nombre con el que se invocan en la web API (los solicitados)

   * NormalOpen -> DaysSchedule. 

      * Afecta a TODOS los días. 

      * Otra llamada machacaría ésta

   * WeekDayOpen -> Days. 

      * Afecta a UN solo día de TODAS las semanas 

      * Se puede llamar varias veces con distintos días

      * Una llamada con un día ya registrado, hace que se actualice el resultado

   * YearDayOpen > Dates. 

      * Afecta a un ÚNICO día

      * Se puede llamar varias veces con distintos días

      * Una llamada con un día ya registrado, hace que se actualice el resultado

   * WeekDaysClose -> DaysClose. 

      * Afecta a UNA LISTA de días de TODAS las semanas

      * Otra llamada machacaría esta lista y se actualizaría el resultado

   * YearDaysClose -> DatesClose.

      * Afecta a UNA LISTA de días de TODAS las semanas

      * Otra llamada machacaría esta lista y se actualizaría el resultado

* y otro método para dar la fecha efectiva de entrega dando los minutos que tarda y la fecha y hora en que se empieza. Se entiende que la precedencia para los intervalos de apertura son de esta forma:

   * Los intervalos de los yearDays prevalecen sobre los de los weekDays en caso de coincidencia

   * Los intervalos de los weekDays prevalecen sobre los de los normalDays en caso de coincidencia

**Asunciones**

Es claro que es un GET para solicitar la fecha de entrega, pero para configurar los horarios no me queda claro si debe ser por web API o debe haber un BackOffice. Si no me decís lo contrario: 

* Lo haré por web API también. 

   * Serán otros 5 endpoints pero contestarán por PUT. 

   * Los pondré en otro controller por si se quiere restringir su uso de forma sencilla mediante algún tipo de autorización

* Si se invoca dos veces la misma API con un mismo día pero distinto intervalo, entiendo que se machaca totalmente la configuración anterior y no se mezclan intervalos. 

* La configuración se debe persistir de alguna forma entre llamadas a las API. JSON, XML, DB... otra opción (la que se implementa) sería hacer un singleton con lifetime de aplicación para que no varíe y nos ahorramos el Load/Save y todo el plumbing de la persistencia, aunque perderíamos la auditoría y habría desafíos a la hora de escalar (y hay gente que considera el singleton un antipatrón)

* El usuario que configura el horario debe tener cuidado en el orden de llamada de las API, ya que puede originar resultados inesperados. Lo mismo pasa cuando dos usuarios remotos acceden simultáneamente a tocar la configuración

* Se implementa adicionalmente un método que da la configuración actual serializando en JSON. También otro que toma ese JSON de una sola vez y lo deserializa

   * Al hacerlo de forma atómica se minimizan los problemas anteriores 

   * Se podría vincular directamente con [FromBody] en el controller. 

   * Puede ser la base para una persistencia

   * Cuidado con no romper el singleton en la deserialización

   * El JSON que se puede meter en la API para configurar el singleton con los valores del ejemplo del enunciado es: "{\"NormalOpen\":{\"Open\":\"09:00:00\",\"Close\":\"15:00:00\"},\"WeekDayOpen\":{\"Friday\":{\"Open\":\"10:00:00\",\"Close\":\"17:00:00\"}},\"YearDayOpen\":{\"2010-12-24\":{\"Open\":\"08:00:00\",\"Close\":\"13:00:00\"}},\"WeekDaysClose\":[0,3],\"YearDaysClose\":[\"2010-12-25\"]}"

* Entendemos que los parámetros están en inglés como se pone en el ejemplo (monday, tuesday...). También hay que tener cuidado con las timezones si aceptamos que se pueda acceder a la aplicación desde diversos lugares del mundo.

* El enunciado dice: "Also, all parameters should be passed by a simple Uri string". Entiendo que es solo para el GET; parece mala práctica solicitar por querystring valores que cambian el estado de la aplicación (como el PUT de la configuración del sitio). Así que usaré en el controller el [FromForm] y generará en Swagger los mismos campos que si se transmite con [FromQuery]. Se puede cambiar fácilmente.

 **\**


**Elaboración y consideraciones**

* He usado el binder de .NET para que lleguen los valores ya preparados a los controllers (los actions del controller tendrán los parámetros listor para usarse, por ejemplo, llegará un datetime aunque el cliente los invoque con string

   * .Net 8 tiene un pequeño bug al mostrar en swagger DateOnly / TimeOnly; le hice un workaround para que funcionase

   * Hay un custom binder para pasar las string csv a List<T>

* Tal vez se debería inicializar con algún valor por defecto al menos el  NormalOpen (DaysSchedule) por si se solicita fecha de entrega de un producto sin haber configurado los tiempos de apertura

* No he dedicado tiempo a la gestión custom de errores, logging, xUnit, authorization... Ni siquiera he testeado fuerte los “edge cases”; solo que funciona el ejemplo. 

