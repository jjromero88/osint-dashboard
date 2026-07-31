import { Injectable } from '@angular/core';
import { Nivel } from '../models/nivel.model';

// Catálogo estático de niveles de profundidad — mismo criterio que
// PaisesService (enum fijo, no vale la pena pedirlo por HTTP). Debe
// mantenerse en sync con Osint.Application.Common.CatalogoNiveles.
const NIVELES: Nivel[] = [
  { valor: 'rapido', nombre: 'Rápido', descripcion: 'Vistazo superficial (~1-2 min)' },
  { valor: 'medio', nombre: 'Medio', descripcion: 'Barrido equilibrado (~4-6 min)' },
  { valor: 'profundo', nombre: 'Profundo', descripcion: 'Todo lo gratuito (hasta ~18 min)' },
];

@Injectable({ providedIn: 'root' })
export class NivelesService {
  getAll(): Nivel[] {
    return NIVELES;
  }
}
