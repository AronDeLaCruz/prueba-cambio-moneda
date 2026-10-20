import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { ConversionService } from '../../core/conversion.service';
import { TiposCambioService } from '../../core/tipos-cambio.service';
import { ConversionResponse, TipoCambio } from '../../core/models';

@Component({
  selector: 'app-conversion',
  imports: [ReactiveFormsModule],
  template: `
    <h2 class="text-xl font-bold text-slate-800">Aplicar tipo de cambio</h2>
    <p class="text-sm text-slate-500">POST /api/conversion · requiere JWT</p>

    <form [formGroup]="form" (ngSubmit)="convertir()" class="mt-4 bg-white rounded-2xl shadow p-6 grid gap-4 md:grid-cols-4">
      <div>
        <label class="text-sm font-medium">Monto</label>
        <input type="number" min="0" step="0.01" formControlName="monto"
          class="mt-1 w-full border rounded-lg px-3 py-2" />
      </div>
      <div>
        <label class="text-sm font-medium">Moneda origen</label>
        <select formControlName="monedaOrigen" class="mt-1 w-full border rounded-lg px-3 py-2">
          @for (m of monedas(); track m) { <option [value]="m">{{ m }}</option> }
        </select>
      </div>
      <div>
        <label class="text-sm font-medium">Moneda destino</label>
        <select formControlName="monedaDestino" class="mt-1 w-full border rounded-lg px-3 py-2">
          @for (m of monedas(); track m) { <option [value]="m">{{ m }}</option> }
        </select>
      </div>
      <div class="flex items-end">
        <button [disabled]="form.invalid || loading()"
          class="w-full bg-blue-600 hover:bg-blue-700 disabled:opacity-50 text-white font-semibold rounded-lg py-2">
          {{ loading() ? 'Convirtiendo...' : 'Convertir' }}
        </button>
      </div>
    </form>

    @if (error()) {
      <p class="mt-4 text-sm text-red-600 bg-red-50 border border-red-200 rounded-lg px-3 py-2">{{ error() }}</p>
    }

    @if (resultado()) {
      <div class="mt-4 bg-green-50 border border-green-200 rounded-2xl p-6">
        <p class="text-sm text-slate-500">Resultado</p>
        <p class="text-3xl font-bold text-slate-800 mt-1">
          {{ resultado()!.monto }} {{ resultado()!.monedaOrigen }} = {{ resultado()!.montoConTipoCambio }} {{ resultado()!.monedaDestino }}
        </p>
        <p class="text-sm text-slate-600 mt-2">Tipo de cambio aplicado: <b>{{ resultado()!.tipoCambio }}</b></p>
      </div>
    }

    @if (tasas().length) {
      <div class="mt-6 bg-white rounded-2xl shadow p-6">
        <h3 class="font-semibold text-slate-700">Tasas registradas (GET /api/tipos-cambio)</h3>
        <ul class="mt-2 text-sm text-slate-600 space-y-1">
          @for (t of tasas(); track t.id) {
            <li>{{ t.monedaOrigen }} → {{ t.monedaDestino }}: <b>{{ t.tasa }}</b></li>
          }
        </ul>
      </div>
    }
  `,
})
export class ConversionComponent implements OnInit {
  private fb = inject(FormBuilder);
  private conversion = inject(ConversionService);
  private tipos = inject(TiposCambioService);

  loading = signal(false);
  error = signal<string | null>(null);
  resultado = signal<ConversionResponse | null>(null);
  tasas = signal<TipoCambio[]>([]);
  monedas = signal<string[]>(['USD', 'PEN', 'EUR']);

  form = this.fb.group({
    monto: [100, [Validators.required, Validators.min(0.01)]],
    monedaOrigen: ['EUR', Validators.required],
    monedaDestino: ['PEN', Validators.required],
  });

  ngOnInit(): void {
    this.tipos.listar().subscribe({
      next: (data) => {
        this.tasas.set(data);
        const set = new Set<string>();
        data.forEach((t) => { set.add(t.monedaOrigen); set.add(t.monedaDestino); });
        if (set.size) this.monedas.set([...set]);
      },
      error: () => undefined,
    });
  }

  convertir(): void {
    if (this.form.invalid) return;
    this.loading.set(true);
    this.error.set(null);
    this.resultado.set(null);
    this.conversion.convertir(this.form.getRawValue() as { monto: number; monedaOrigen: string; monedaDestino: string }).subscribe({
      next: (res) => { this.loading.set(false); this.resultado.set(res); },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        this.error.set((err.error as { message?: string })?.message ?? 'No se pudo convertir. Verifica las monedas.');
      },
    });
  }
}
