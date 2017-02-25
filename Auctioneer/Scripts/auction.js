var Auction = {

  initialize: function(auctionId) {
    this.setupSignalr(auctionId);
  },

  setupSignalr: function (auctionId) {
    if (auctionId === undefined) {
      throw "Auction ID must be provided for JS functionality";
    }

    var bidHub = $.connection.bidHub;

    bidHub.client.updateBidAmount = function (amount) {
      var $amountField = $("#bidAmount");

      if ($amountField.hasClass("text-success")) {
        $amountField.removeClass("text-success").addClass("text-animation-outbid");
      }

      $("#bidAmount").text(amount);
    };

    $.connection.hub.start().done(function () {
      bidHub.server.registerClient(auctionId);
    });
  },

  updateTime: function() {
    var endDate = $("#endTime").data("time"),
        timeStr = "",
        t = Date.parse(endDate) - Date.parse(new Date()),
        seconds = Math.floor((t / 1000) % 60),
        minutes = Math.floor((t / 1000 / 60) % 60),
        hours = Math.floor((t / (1000 * 60 * 60)) % 24),
        days = Math.floor(t / (1000 * 60 * 60 * 24));

    if (days > 0)
      timeStr += days + "d ";
    if (hours > 0)
      timeStr += hours + "h ";
    if (minutes > 0)
      timeStr += minutes + "m ";
    if (seconds > 0)
      timeStr += seconds + "s ";

    $("#endTime").text("Ending in: " + timeStr);
  }

};