var KENDOFOLDER = 'Q1.2016';

$(function () {

    var theme = ida.common.getCookie("IDA-Theme");
    navIndex = ida.common.getCookie("IDA-NavIndex");

    if (theme != "") {
        ida.core.setTheme(theme);
    }

    var path = window.location.pathname;

    if (path != "/") {
        window.setTimeout(ida.core.navMenuLoad, 1000);
    }

});


ida.core =

      {
          navMenuSelect: function navMenuSelect(e) {

              // Store Nav Index.

              var item = $(e.item),
              index = item.parentsUntil(".k-panelbar", ".k-item").map(function () {
                  return $(this).index();
              }).get().reverse();

              index.push(item.index());

              ida.common.setCookie('IDA-NavIndex', index, 365);

              // Handle Navigation

              var path = e.item.id;

              switch (path) {

                  case "None":
                      break;

                  case "TeamCity":

                      window.location.href = "http://c1devweb01.axiomainc.com:8090/";
                      break;

                  case "JIRA":

                      window.location.href = "http://c1prdweb06.conceptonellc.com:8080/secure/Dashboard.jspa";
                      break;

                  case "Confluence":

                      window.location.href = "http://c1prdweb06.conceptonellc.com:8090/dashboard.action";
                      break;

                  default:

                      $("main").load(path);

                      var stateObj = { path: path };
                      history.pushState(stateObj, path, path);
                      break;
              }
          },

          navMenuLoad: function navMenuLoad() {

              var target = "0";

              if (navIndex != "") {
                  target = navIndex;
                  //alert(navIndex);
              }

              var panelbar = $("#nav-menu-left").data("kendoPanelBar");

              var itemIndexes = target.split(/[.,]/),
              rootItem = panelbar.element.children("li").eq(itemIndexes[0]);

              var getItem = itemIndexes.length > 1 ?
                     rootItem.find(".k-group > .k-item").eq(itemIndexes[1]) :
                     rootItem;

              panelbar.expand(rootItem);

              getItem.attr("aria-selected", "true");

              if (itemIndexes.length > 1) {
                  itemSpan = getItem.find("span");
                  itemSpan.attr("class", "k-link k-state-selected");
              }

          },

          setTheme: function setTheme(themeName) {

              //alert('Setting Theme: ' + themeName)

              switch (themeName) {

                  case "Black":
                      $('#kendoStyle').attr('href', '/Content/Kendo/' + KENDOFOLDER + '/kendo.black.min.css');
                      $('#header-main').attr('style', 'background-color: #272727;');
                      ida.common.setCookie('IDA-Theme', themeName, 365);
                      break;
                  case "BlueOpal":
                      $('#kendoStyle').attr('href', '/Content/Kendo/' + KENDOFOLDER + '/kendo.blueopal.min.css');
                      $('#header-main').attr('style', 'background-color: #0d4e90;');
                      // $('#userName').attr('style', 'color: white;');
                      ida.common.setCookie('IDA-Theme', themeName, 365);
                      break;
                  case "Bootstrap":
                      $('#kendoStyle').attr('href', '/Content/Kendo/' + KENDOFOLDER + '/kendo.bootstrap.min.css');
                      $('#header-main').attr('style', 'background-color: #0d4e90;');
                      ida.common.setCookie('IDA-Theme', themeName, 365);
                      break;
                  case "Default":
                      $('#kendoStyle').attr('href', '/Content/Kendo/' + KENDOFOLDER + '/kendo.default.min.css');
                      $('#header-main').attr('style', 'background-color: #0d4e90;');
                      ida.common.setCookie('IDA-Theme', themeName, 365);
                      break;
                  case "Flat":
                      $('#kendoStyle').attr('href', '/Content/Kendo/' + KENDOFOLDER + '/kendo.flat.min.css');
                      $('#header-main').attr('style', 'background-color: #272727;');
                      ida.common.setCookie('IDA-Theme', themeName, 365);
                      break;
                  case "Fiori":
                      $('#kendoStyle').attr('href', '/Content/Kendo/' + KENDOFOLDER + '/kendo.fiori.min.css');
                      $('#header-main').attr('style', 'background-color: #0d4e90;');
                      ida.common.setCookie('IDA-Theme', themeName, 365);
                      break;
                  case "HighContrast":
                      $('#kendoStyle').attr('href', '/Content/Kendo/' + KENDOFOLDER + '/kendo.highcontrast.min.css');
                      $('#header-main').attr('style', 'background-color: black;');
                      ida.common.setCookie('IDA-Theme', themeName, 365);
                      break;
                  case "Metro":
                      $('#kendoStyle').attr('href', '/Content/Kendo/' + KENDOFOLDER + '/kendo.metro.min.css');
                      $('#header-main').attr('style', 'background-color: #0d4e90;');
                      ida.common.setCookie('IDA-Theme', themeName, 365);
                      break;
                  case "MetroBlack":
                      $('#kendoStyle').attr('href', '/Content/Kendo/' + KENDOFOLDER + '/kendo.metroblack.min.css');
                      $('#header-main').attr('style', 'background-color: black;');
                      ida.common.setCookie('IDA-Theme', themeName, 365);
                      break;
                  case "Moonlight":
                      $('#kendoStyle').attr('href', '/Content/Kendo/' + KENDOFOLDER + '/kendo.moonlight.min.css');
                      $('#header-main').attr('style', 'background-color: black;');
                      ida.common.setCookie('IDA-Theme', themeName, 365);
                      break;
                  case "Silver":
                      $('#kendoStyle').attr('href', '/Content/Kendo/' + KENDOFOLDER + '/kendo.silver.min.css');
                      $('#header-main').attr('style', 'background-color: #0d4e90;');
                      ida.common.setCookie('IDA-Theme', themeName, 365);
                      break;
                  case "Uniform":
                      $('#kendoStyle').attr('href', '/Content/Kendo/' + KENDOFOLDER + '/kendo.uniform.min.css');
                      $('#header-main').attr('style', 'background-color: #0d4e90;');
                      ida.common.setCookie('IDA-Theme', themeName, 365);
                      break;
                  case "Nova":
                      $('#kendoStyle').attr('href', '/Content/Kendo/' +KENDOFOLDER + '/kendo.nova.min.css');
                      $('#header-main').attr('style', 'background-color: #0d4e90;');
                      ida.common.setCookie('IDA-Theme', themeName, 365);
                      break;

                  default:
                      break;
              }

          },

          quickActionMenuOnClick: function quickActionMenuOnClick(e) {

              var linkId = e.item.id;

              switch (linkId) {

                  case "NONE":
                      break;
                      // case "ALERTS":
                  default:
                      ida.common.displayMessage({ messageType: 'info-development' });
                      break;
              }
          },

          getUserObj: function getUserObj() {

              var userId = $('#userId').text();
              var userRole = $('#userRole').text();

              return {
                  userId: userId,
                  userRole: userRole
              }
          },

          userMenuSelect: function userMenuSelect(e) {

              var selectId = e.item.id;

              switch (selectId) {

                  case "PROFILE":

                      window.location.assign("/Home/Index");

                      break;

                  case "THEME":

                      var linkText = $(e.item).children(".k-link").text();
                      ida.core.setTheme(linkText);

                      break;

                  case "IDA-HOME":

                      window.location.assign("/Home/Index");

                      break;

                  default:

                      break;
              }
          }

      }

demi = {
    stepCount: 0,
    validationError: false,
    sourceFilePath: '',
    destinationFilePath: '',
    transSetId: 0,
    transSetName: 'Client_XYZ_for_PLD_table',
    fileChangeOccured: false,
    transformationRuleChangeOccured: true,  //TODO: set to false when grid is dirty
    }

ida.core.demi =

      {
          initializeTransformation: function initializeTransformation() {

              demi.stepCount = 0;

              transProgress = $("#transformationProgress").data("kendoProgressBar");
              transProgress.progressStatus.text("File Selection");
              $("#transHeader").text('File Selection:  Select Transformation Set Name, Source and Destination files.');

              $("#transStep2").hide();
              $("#transStep3").hide();
          },

          onTransformationChange: function onTransformationChange(e) {

              this.progressWrapper.css({
                  "background-image": "none",
                  "border-image": "none"
              });
              
              if (e.value < 1) {
                  this.progressStatus.text("File Selection");

                  if (!demi.validationError == true) {
                      demi.fileChangeOccured = false;
                      demi.validationError == false;
                  }

                  $("#transHeader").text('File Selection:  Select Transformation Set Name, Source and Destination files.');

                  $("#transStep1").show();
                  $("#transStep2").hide();
                  $("#transStep3").hide();

                  $("#nextButton").show();

                  $("#nextButton").css("float", "left");
                  $("#previousButton").css("visibility", "hidden");
                  $("#cancelButton").css("visibility", "hidden");

              } else if (e.value == 1) {

                  var transformationSet = $("#transformationSet").data("kendoAutoComplete");

                  if (transformationSet.value() == '') {
                      // alert(transformationSet.value());
                      ida.common.displayMessage({ messageType: 'info', messageText: "A Transformation Set must be provided." });
                      e.preventDefault();
                      demi.validationError = true;
                      ida.core.demi.transformationNextStep(demi.stepCount -= 1);
                  }
                  else {
                      demi.transSetName = transformationSet.value();
                  }

                  if (!e.isDefaultPrevented()) {
            
                          this.progressStatus.text("Transformation Mapping");

                          if (demi.fileChangeOccured) {
                              $.when(ida.core.demi.createTransformationSet()).done(function(x) {
                              //alert('file change occurred.');
                                  $("#sourceGrid").data('kendoGrid').dataSource.read();
                                  $("#transformationGrid").data('kendoGrid').dataSource.read();
                                  demi.fileChangeOccured = false;
                              });
                          }

                          $("#transHeader").text("Transformation Mapping:  Enter Transformation Rules, click 'Save changes'.");

                          $("#transStep1").hide();
                          $("#transStep2").show();
                          $("#transStep3").hide();

                          $("#nextButton").show();
                          // $("#nextButton").text("Next Step");

                          $("#nextButton").css("float", "none");
                          $("#previousButton").css("visibility", "visible");
                          $("#cancelButton").css("visibility", "visible");

                          this.progressWrapper.css({
                              "background-color": "#428bca",
                              "border-color": "#428bca"
                          });

                          //this.progressWrapper.css({
                          //    "background-color": "#EE9F05",
                          //    "border-color": "#EE9F05"
                              //});

                   }

              } else if (e.value == 2) {
                  
                  this.progressStatus.text("Transformation Results");

                  demi.fileChangeOccured = false;
                 
                  // $("#nextButton").text("Confirm");

                  $("#nextButton").hide();

                  $("#transStep2").hide();
                  $("#transStep3").show();

                  $("#transFileDownload").hide();

                  if (demi.transformationRuleChangeOccured) {
                      $("#transHeader").text('Transformation Results: Your transformation was successful.');
                      ida.core.demi.loadTransformationResults();
                      $("#transFileDownload").show();
                      // ida.common.displayMessage({ messageType: 'info', messageText: "Your transformation was successful." });
                  }
                  else {
                      $("#transHeader").text('Transformation Results: Your transformation failed.');
                      ida.common.displayMessage({ messageType: 'error', messageText: "No transformation rules found." });
                  }

                  // demi.transformationRuleChangeOccured = false;

                  this.progressWrapper.css({
                      "background-color": "#8EBC00",
                      "border-color": "#8EBC00"
                  });

              } else {

                  //this.progressStatus.text("Transformation Complete");

                  //$("#transHeader").text('Transformation Complete:  Your transformation has completed successfully.');

                  //$("#transStep3").hide();

                  //$("#nextButton").css("visibility", "hidden");
                  //$("#previousButton").css("visibility", "hidden");
                  //$("#cancelButton").css("visibility", "hidden");

                  //this.progressWrapper.css({
                  //    "background-color": "#8EBC00",
                  //    "border-color": "#8EBC00"
                  //});
              }
          },

          onTransformationGridSave: function onTransformationGridSave(e) {
          
              demi.transformationRuleChangeOccured = true;
          },

          onGridError: function onGridError(e) {
             
              if (e.errors) {
                  var message = "Errors:\n";
                  $.each(e.errors, function (key, value) {
                      if ('errors' in value) {
                          $.each(value.errors, function () {
                              message += this + "\n";
                          });
                      }
                  });
                  ida.common.displayMessage({ messageType: 'error', messageText: message });
              }                

          },

          createTransformationSet: function createTransformationSet() {
                         
              return $.ajax({
                  url: "/DEMI/CreateTransformationSet",
                  datatype: "json",
                  data: { 'setName': demi.transSetName, 'fileName': demi.destinationFilePath },
                  type: "POST",
                  success: function (data) {
                      demi.transSetId = data;
                  },
                  error: function (data) {
                      ida.common.displayMessage({ messageType: 'error', messageText: data.error });
                      // ida.common.displayMessage({ messageType: 'error', messageText: "There was an error creating this transformation set." });
                  }
              });
          },

          loadTransformationResults: function loadTransformationResults() {
                           
              // TODO: Phase II.
              // var spreadsheet = $("#transformationResultsSheet").data("kendoSpreadsheet");
                           
              $("#transFileDownload").hide();

                $("#transFileDownload").click(function () {
                    window.location = '/DEMI/DownloadTransformedData?setId=' +demi.transSetId;
                });

          },

          transformationNextStep: function transformationNextStep(stepCount) {

              transProgress = $("#transformationProgress").data("kendoProgressBar");

              transProgress.value(stepCount);

          },

          transformationCancel: function transformationCancel(stepCount) {

              $("main").load("/DEMI/Transformation");

          },

          onFileSelect: function onFileSelect(e) {

              var uploadId = this.name;

              $("#nextButton").css("visibility", "hidden");

              if (e.files.length > 1) {
                  ida.common.displayMessage({ messageType: 'info', messageText: "Please select only 1 file." });
                  e.preventDefault();
                  alert('length > 1');
              }

              $.each(e.files, function () {
                  if (this.extension.toLowerCase() != ".csv" && this.extension.toLowerCase() != ".xlsx") {
                      ida.common.displayMessage({ messageType: 'info', messageText: "Only CSV (.csv) and Excel 2007/2010 (.xlsx) files are supported." });
                      e.preventDefault();
                      alert('file not supported');
                  } else {
                      if (uploadId == "sourceFiles") {
                          demi.sourceFilePath = this.name;
                      }
                      else {
                          demi.destinationFilePath = this.name;
                      }

                      demi.fileChangeOccured = true;
                  }
              });

              if (!e.isDefaultPrevented()) {
                  var sourceFilesUpload = $("#transTep1-sourceFiles").find("ul");
                  var destinationFilesUpload = $("#transTep1-destinationFiles").find("ul");

                  // alert(sourceFilesUpload.length);

                  if (uploadId == "destinationFiles" && sourceFilesUpload.length > 0) {
                      $("#nextButton").css("visibility", "visible");
                  }
                  else if (uploadId == "sourceFiles" && destinationFilesUpload.length > 0) {
                      $("#nextButton").css("visibility", "visible");
                  }
              }
          },

          onFileRemove: function onFileRemove(e) {

              $("#nextButton").css("visibility", "hidden");

          },

          onFileSelectError: function onFileSelectError(e) {

              var err = e.XMLHttpRequest.responseText;

              ida.common.displayMessage({ messageType: 'error', messageText: "An error occured during the " + e.operation + " operation:</br>" + err });
              e.preventDefault();
          },

          getFileInfo: function getFileInfo(e) {
              return $.map(e.files, function (file) {
                  var info = file.name;

                  // File size is not available in all browsers
                  if (file.size > 0) {
                      info += " (" + Math.ceil(file.size / 1024) + " KB)";
                  }
                  return info;
              }).join(", ");
          },

          addExtensionClass: function addExtensionClass(extension) {
              switch (extension) {
                  case '.jpg':
                  case '.img':
                  case '.png':
                  case '.gif':
                      return "img-file";
                  case '.doc':
                  case '.docx':
                      return "doc-file";
                  case '.xls':
                  case '.xlsx':
                      return "xls-file";
                  case '.pdf':
                      return "pdf-file";
                  case '.zip':
                  case '.rar':
                      return "zip-file";
                  default:
                      return "default-file";
              }
          },

          getSourceGridOptions: function getSourceGridOptions() {
                          
              return {
                  fileName: demi.sourceFilePath
              }
          },

          getTransformationGridOptions: function getTransformationGridOptions() {

              return {
                  fileName: demi.destinationFilePath,
                  setId: demi.transSetId
              }
          },

          loadSourceData: function loadSourceData(fileName) {
              var dataSrc = getSourceDataSource(fileName);
              //var dataTable = bindSourceGrid(dataSrc);
          },

          onAdditionalData: function onAdditionalData() {
              return {
                  text: $("#transformationSet").val()
              };
          }

      }