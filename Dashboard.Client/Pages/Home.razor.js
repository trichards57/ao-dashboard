let pieChart;
let historicChart;

function dateLabel(d) {
  const date = new Date(d);

  const months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
  const month = months[date.getMonth()];
  const year = date.getFullYear() - 2000;

  return `${month} ${year}`;
}

window.addEventListener('resize', function () {
  pieChart?.resize();
  historicChart?.resize();
});

export function loadGraphs(data) {
  const pieCtx = document.getElementById('vehicle-availability-chart').getContext('2d');

  if (pieChart && !pieChart.attached) {
    pieChart.destroy();
    pieChart = undefined;
  }

  if (!pieChart) {
    pieChart = new Chart(pieCtx, {
      type: 'doughnut',
      data: {
        labels: ['Available', 'VOR'],
        datasets: [{
          label: 'Vehicle Status',
          data: [data.availableVehicles, data.vorVehicles],
          backgroundColor: ['#007a53', '#97999b'],
        }]
      },
      options: {
        responsive: true,
        circumference: 180,
        rotation: -90,
        cutout: '75%', // Adjust this to change the thickness of the arc
        plugins: {
          legend: {
            display: false
          },
        },
        aspectRatio: 1 // Keeps the chart as a circle on all screen sizes
      }
    });
  }
  else {
    pieChart.data.datasets[0].data = [data.availableVehicles, data.vorVehicles];
    pieChart.update();
  }

  const lineCtx = document.getElementById('historic-availability-chart').getContext('2d');

  if (historicChart && !historicChart.attached) {
    historicChart.destroy();
    historicChart = undefined;
  }

  if (!historicChart) {
    historicChart = new Chart(lineCtx, {
      type: 'line',
      data: {
        labels: Object.keys(data.pastAvailability).map(dateLabel),
        datasets: [{
          label: 'Available Vehicles',
          data: Object.values(data.pastAvailability),
          fill: false,
        }]
      },
      options: {
        responsive: true,
        plugins: {
          legend: {
            display: false
          },
        },
      }
    });
  }
  else {
    historicChart.data.labels = Object.keys(data.pastAvailability).map(dateLabel);
    historicChart.data.datasets[0].data = Object.values(data.pastAvailability);
    historicChart.update();
  }
}
