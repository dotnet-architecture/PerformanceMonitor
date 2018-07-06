interface DataTransfer;

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/metricHub")
    .build();

connection.on("CPU", cpu => {

}