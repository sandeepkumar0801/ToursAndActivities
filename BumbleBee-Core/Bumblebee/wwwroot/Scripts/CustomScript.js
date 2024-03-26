(function () {
    window.addEventListener("load", function () {
        setTimeout(function () {
            // Fetch data from API endpoint
            fetch('/api/GetConfig')
                .then(response => response.json())
                .then(data => {
                    // Section 03 - Set env value
                    var envValue = data.EnvValue;
                    console.log('Environment value:', envValue);

                    // Section 01 - Set url link 
                    var logo = document.getElementsByClassName('link');
                    logo[0].href = "https://www.isango.com/";
                    logo[0].target = "_blank";

                    // Section 02 - Set logo
                    logo[0].children[0].alt = "IsangoAPI";
                    logo[0].children[0].src = "https://hohobassets.isango.com/phoenix/images/isango-cs.png";

                    // Section 03 - Use env value as needed
                    // ...

                })
                .catch(error => console.error('Error fetching config:', error));
        });
    });
})();

