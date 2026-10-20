import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { TiposCambioService } from '../../core/tipos-cambio.service';
import { TipoCambio } from '../../core/models';
import { DatePipe } from '@angular/common';
import { getErrorMessage } from '../../core/error-message';

@Component({
  selector: 'app-tipos-cambio',
  imports: [ReactiveFormsModule, DatePipe],
  templateUrl: './tipos-cambio.component.html',
})
export class TiposCambioComponent implements OnInit {
  private fb = inject(FormBuilder);
  private service = inject(TiposCambioService);

  items = signal<TipoCambio[]>([]);
  loading = signal(false);
  saving = signal(false);
  mostrarForm = signal(false);
  editId = signal<number | null>(null);
  msg = signal<string | null>(null);
  esError = signal(false);

  form = this.fb.group({
    monedaOrigen: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(3)]],
    monedaDestino: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(3)]],
    tasa: [0, [Validators.required, Validators.min(0.0001)]],
  });

  ngOnInit(): void {
    this.cargar();
  }

  cargar(): void {
    this.loading.set(true);
    this.service.listar().subscribe({
      next: (d) => { this.loading.set(false); this.items.set(d); },
      error: (e: HttpErrorResponse) => { this.loading.set(false); this.fail('No se pudo listar: ' + this.detail(e)); },
    });
  }

  nuevo(): void {
    this.editId.set(null);
    this.form.reset({ monedaOrigen: '', monedaDestino: '', tasa: 0 });
    this.mostrarForm.set(true);
  }

  editar(t: TipoCambio): void {
    this.editId.set(t.id);
    this.form.setValue({ monedaOrigen: t.monedaOrigen, monedaDestino: t.monedaDestino, tasa: t.tasa });
    this.mostrarForm.set(true);
  }

  cancelar(): void {
    this.mostrarForm.set(false);
    this.editId.set(null);
  }

  guardar(): void {
    if (this.form.invalid) return;
    this.saving.set(true);
    const raw = this.form.getRawValue();
    const body = {
      monedaOrigen: (raw.monedaOrigen ?? '').toUpperCase(),
      monedaDestino: (raw.monedaDestino ?? '').toUpperCase(),
      tasa: Number(raw.tasa),
    };
    const id = this.editId();
    const req = id ? this.service.actualizar(id, body) : this.service.crear(body);
    req.subscribe({
      next: () => { this.saving.set(false); this.mostrarForm.set(false); this.ok(id ? 'Actualizado' : 'Creado'); this.cargar(); },
      error: (e: HttpErrorResponse) => { this.saving.set(false); this.fail('No se pudo guardar: ' + this.detail(e)); },
    });
  }

  eliminar(t: TipoCambio): void {
    if (!confirm('Eliminar ' + t.monedaOrigen + ' -> ' + t.monedaDestino + '?')) return;
    this.service.eliminar(t.id).subscribe({
      next: () => { this.ok('Eliminado'); this.cargar(); },
      error: (e: HttpErrorResponse) => this.fail('No se pudo eliminar: ' + this.detail(e)),
    });
  }

  private ok(m: string): void { this.esError.set(false); this.msg.set(m); }
  private fail(m: string): void { this.esError.set(true); this.msg.set(m); }
  private detail(e: HttpErrorResponse): string {
    return getErrorMessage(e);
  }
}
