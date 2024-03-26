(function () {
    window.addEventListener("load", function () {
        setTimeout(function () {
            var tempContainer = document.createElement("div");
            var fragment = document.createDocumentFragment();

            tempContainer.innerHTML = `
            <div id='swagger-ui-containers' class='swagger-ui-wrap'> <div class=inf' id='api_info'> <div class='info_title'></div>  
            <div class='info_description markdown'></div></div> 
            <div class='container' id='resources_container'> 
              <div class='opblock-tag-section'></div> <ul id='resources'><li id='resource_Token' class='resource'><div class='heading'> <h2> 
             <a href='#' class='expand-operation'  data-id='Activity'>Token</a> </h2> 
              <ul class='options'> 
              <li> <a href='#' id='endpointListTogger_Token' class='toggleEndpointList' data-id='Activity' data-sw-translate=''>Show/Hide</a></li> 
              <li> <a href='#' class='expandResource_Token' data-id='Token' data-sw-translate=''> Expand Operations </a> 
              </li> 
              </ul> 
            </div> 
            <ul class='endpoints' id='Token_endpoint_list' style='display:none'> 
             <li class='endpoint'>  <ul class='operations'> 
                <li class='post operation' id='Token_Token_GetToken'> 
                  <div class='heading'> 
                    <h3> 
                      <span class='http_method'> 
                      <a href='#' class='toggleOperation'>post</a> 
                      </span> 
                      <span class='path'> 
                      <a href='#' class='toggleOperation  expandResource_Token'>/Token</a> 
                      </span> 
                    </h3> 
                    <ul class='options'> 
                      <li> 
                      <a href='#' class='toggleOperation'><span class='markdown'></span></a> 
                      </li> 
                    </ul> 
                  </div> 
                  <div class='content' id='Token_Token_Token_content' style='display:none'> 
                  <form accept-charset='UTF-8' class='sandbox'> 
                      <div style='margin:0;padding:0;display:inline'></div> 
                      <h4 data-sw-translate=''>Parameters</h4> 
                      <table class='fullwidth parameters'> 
                      <thead> 
                        <tr> 
                        <th style='width: 100px; max-width: 100px' data-sw-translate=''>Parameter</th> 
                        <th style='width: 310px; max-width: 310px' data-sw-translate=''>Value</th> 
                        <th style='width: 200px; max-width: 200px' data-sw-translate=''>Parameter</th> 
                        <th style='width: 100px; max-width: 100px' data-sw-translate=''>Value Type</th> 
                        </tr> 
                      </thead> 
                      <tbody class='operation-params'> 
             <tr><td class='code required'><label >UserName</label></td> 
            <td> <input placeholder='username' id='input_username' name='username' type='text' placeholder='(required)' size='10'></td> 
            <td class='code required'> <label >Password</label></td><td><input placeholder='password' id='input_password' name='password' type='password' size='10'></td> 
            <td> </div></div></div></span></td> 
            </tr> </tbody> 
                      </table> 
                      <div class='sandbox_header'> 
                        <input class='submit' value='Try it out!' data-sw-translate='' onclick='GetToken()' type='button'> 
                        <a href='#' class='response_hider' style='display:none' data-sw-translate=''>Hide Response</a> 
                        <span class='response_throbber' style='display:none'></span> 
                      </div> 
                    </form> <div class='responseToken' style='display:none'> 
             <div class='response'> 
                      <h4 data-sw-translate=''>Request URL</h4> 
                      <div class='block request_url_token'></div> 
                      <h4 data-sw-translate=''>Response Body</h4> 
                      <div class='block response_bodyToken json'></div> 
                      <h4 data-sw-translate=''>Response Code</h4> 
                      <div class='block response_code_token'></div> 
                      <h4 data-sw-translate=''>Response Headers</h4> 
                      <div class='block response_headers_token'></div> 
            </div> </div> 
            </div> 
                  </div> 
                </li> 
              </ul> 
            </li></ul> 
            </ul> 
           </div> </div>
          `;

            while (tempContainer.firstChild) {
                fragment.appendChild(tempContainer.firstChild);
            }

            // Find all the elements with class 'swagger-ui'
            var swaggerUiElements = document.querySelectorAll(".swagger-ui");

            // Check if there are at least two 'swagger-ui' elements
            if (swaggerUiElements.length >= 2) {
                // Get the second occurrence of the 'swagger-ui' element
                var targetSwaggerUiElement = swaggerUiElements[1];

                // Insert the custom HTML before the second 'swagger-ui' element
                targetSwaggerUiElement.parentNode.insertBefore(fragment, targetSwaggerUiElement);
            } else {
                console.error("There are not enough occurrences of the element with class 'swagger-ui'.");
            }


            var scriptElement = document.createElement("script");
            scriptElement.textContent = `
       var GetToken = function () {
    var username = $('#input_username').val();
    var password = $('#input_password').val();
    if (username && username.trim() != "" && password && password.trim() != "") {
        $('.response_throbber').css("display", "block");

        var Token = '';
        $.ajax({
            url: "/token",
            type: "POST",
            data: {
                grant_type: 'password'.trim(),
                username: $('#input_username').val(),
                password: $('#input_password').val()
            },
            success: function (data, status, xhr) {
                Token = data.access_token;
                var ResponseHtml = "<pre class='json'><code> Bearer " + Token + "</code></pre>"
                $('.response_bodyToken').empty().append(ResponseHtml);
                $('.request_url_token').empty().append("<pre>" + document.location.origin + "/token" + "</pre>");
                $('.response_code_token').empty().append("<pre>" + xhr.status + "</pre>");
                $('.response_headers_token').empty().append("<pre>" + xhr.getAllResponseHeaders() + "</pre>");

                $(".responseToken").css("display", "block");
                $('.response_throbber').css("display", "none");
            },
            error: function (xhr, status, error) {
                console.log(xhr);
                console.log(status);
                console.log(error);

                $('.response_throbber').css("display", "none");
                var ResponseHtml = "<pre class='json'><code>" + error + "</code></pre>"
                $('.response_bodyToken').empty().append(ResponseHtml);
                $('.request_url_token').empty().append("<pre>" + document.location.origin + "/token" + "</pre>");
                $('.response_code_token').empty().append("<pre>" + xhr.status + "</pre>");
                $('.response_headers_token').empty().append("<pre>" + xhr.getAllResponseHeaders() + "</pre>");
                $(".responseToken").css("display", "block");
            }
        });
    }
    else {
        if (username.trim() == "") {
            $('#input_username').addClass('parameter required error');
        }
        if (password.trim() == "") {
            $('#input_password').addClass('parameter required error');
        }
    }
};

$('#endpointListTogger_Token').click(function () {
    if ($('#Token_endpoint_list').css('display') == 'none') {
        $("#Token_endpoint_list").css("display", "block");
    }
    else {
        $("#Token_endpoint_list").css("display", "none");
    }
    $('.expandResource_Token').click(function () {
        if ($('#Token_Token_Token_content').css('display') == 'none') {
            $("#Token_Token_Token_content").css("display", "block");
            $("#Token_endpoint_list").css("display", "block");
        }
        else {
            $("#Token_Token_Token_content").css("display", "none");
            $("#Token_endpoint_list").css("display", "none");
        }
    });
});
   `;

            document.head.appendChild(scriptElement);
        });
    });
})();
