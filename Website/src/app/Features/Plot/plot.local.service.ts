import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PlotLocalService {
  constructor() {}

  getPlot(m: number, a: number, p: number, e: number, baseCurrency: string) {
    return {
      data: [
        {
          x: this.getX(m),
          y: this.getY(m, a, p, e),
          type: 'scatter',
          mode: 'lines+markers+text',
          marker: { color: 'black' },
        },
      ],
      layout: {
        autosize: true,
        // width: 820,
        // height: 400,
        xaxis: {
          title: {
            text: 'Miesiąc',
          },
        },
        yaxis: {
          title: {
            text: `Flow (kasa) ${baseCurrency}`,
          },
        },
        showlegend: false,
        pierdola: 12,
        paper_bgcolor: 'white',
      },
    };
  }

  getX(m: number) {
    let result = [];

    for (let i = 0; i < m; i++) {
      result.push(i);
    }

    return result;
  }

  getY(m: number, a: number, p: number, e: number) {
    let result = [];

    for (let i = 0; i < m; i++) {
      result.push(this.flowFunction(i, a, p, e));
    }

    return result;
  }

  flowFunction(m: number, a: number, p: number, e: number) {
    return a + m * (p - e);
  }
}
