function setCount(sheet, val) {
  sheet.insertRows(2, 1);
  sheet.getRange(2, 1).setValue(val);
  sheet.getRange(2, 2).setValue(new Date());

}

function doPost(e) {
  var sheet = SpreadsheetApp.getActiveSpreadsheet().getSheetByName('シート1');
  var params = JSON.parse(e.postData.getDataAsString());
  var val = params.count;

  // データ追加
  setCount(sheet, val);

  var output = ContentService.createTextOutput();
  output.setMimeType(ContentService.MimeType.JSON);
  output.setContent(JSON.stringify({ message: "success!" }));

  return output;
}