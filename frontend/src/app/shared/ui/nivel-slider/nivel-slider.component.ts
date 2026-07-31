import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { Nivel } from '../../models/nivel.model';

// Slider temático de 3 pasos para "nivel" (rápido/medio/profundo) — mismo
// contrato de string que antes tenía el <select>, solo cambia el widget.
@Component({
  selector: 'osint-nivel-slider',
  imports: [],
  templateUrl: './nivel-slider.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NivelSliderComponent {
  @Input() niveles: Nivel[] = [];
  @Input() valor = 'medio';
  @Output() valorChange = new EventEmitter<string>();

  protected get indice(): number {
    const i = this.niveles.findIndex((n) => n.valor === this.valor);
    return i === -1 ? 0 : i;
  }

  protected get nivelActual(): Nivel | undefined {
    return this.niveles[this.indice];
  }

  protected get porcentaje(): number {
    return this.niveles.length <= 1 ? 0 : (this.indice / (this.niveles.length - 1)) * 100;
  }

  protected onInput(event: Event): void {
    this._emitirIndice(Number((event.target as HTMLInputElement).value));
  }

  protected seleccionar(index: number): void {
    this._emitirIndice(index);
  }

  private _emitirIndice(index: number): void {
    const nivel = this.niveles[index];
    if (nivel) this.valorChange.emit(nivel.valor);
  }
}
