import { Injectable } from '@angular/core';
import { Pais } from '../models/pais.model';

// Lista estática de códigos de discado (E.164) — no hay catálogo de países
// en el backend, y no tiene sentido pedirlo por HTTP (dato universal, no de
// negocio). Centralizada acá para no hardcodearla en ningún componente.
// Perú primero (contexto principal del proyecto), resto alfabético.
const PAISES: Pais[] = [
  { codigo: '+51', nombre: 'Perú' },
  { codigo: '+54', nombre: 'Argentina' },
  { codigo: '+61', nombre: 'Australia' },
  { codigo: '+43', nombre: 'Austria' },
  { codigo: '+32', nombre: 'Bélgica' },
  { codigo: '+591', nombre: 'Bolivia' },
  { codigo: '+55', nombre: 'Brasil' },
  { codigo: '+1', nombre: 'Canadá / Estados Unidos' },
  { codigo: '+56', nombre: 'Chile' },
  { codigo: '+86', nombre: 'China' },
  { codigo: '+57', nombre: 'Colombia' },
  { codigo: '+506', nombre: 'Costa Rica' },
  { codigo: '+53', nombre: 'Cuba' },
  { codigo: '+45', nombre: 'Dinamarca' },
  { codigo: '+593', nombre: 'Ecuador' },
  { codigo: '+20', nombre: 'Egipto' },
  { codigo: '+503', nombre: 'El Salvador' },
  { codigo: '+34', nombre: 'España' },
  { codigo: '+33', nombre: 'Francia' },
  { codigo: '+49', nombre: 'Alemania' },
  { codigo: '+502', nombre: 'Guatemala' },
  { codigo: '+504', nombre: 'Honduras' },
  { codigo: '+91', nombre: 'India' },
  { codigo: '+62', nombre: 'Indonesia' },
  { codigo: '+353', nombre: 'Irlanda' },
  { codigo: '+972', nombre: 'Israel' },
  { codigo: '+39', nombre: 'Italia' },
  { codigo: '+81', nombre: 'Japón' },
  { codigo: '+52', nombre: 'México' },
  { codigo: '+31', nombre: 'Países Bajos' },
  { codigo: '+505', nombre: 'Nicaragua' },
  { codigo: '+234', nombre: 'Nigeria' },
  { codigo: '+47', nombre: 'Noruega' },
  { codigo: '+507', nombre: 'Panamá' },
  { codigo: '+595', nombre: 'Paraguay' },
  { codigo: '+63', nombre: 'Filipinas' },
  { codigo: '+48', nombre: 'Polonia' },
  { codigo: '+351', nombre: 'Portugal' },
  { codigo: '+1', nombre: 'Puerto Rico' },
  { codigo: '+44', nombre: 'Reino Unido' },
  { codigo: '+7', nombre: 'Rusia' },
  { codigo: '+65', nombre: 'Singapur' },
  { codigo: '+27', nombre: 'Sudáfrica' },
  { codigo: '+46', nombre: 'Suecia' },
  { codigo: '+41', nombre: 'Suiza' },
  { codigo: '+66', nombre: 'Tailandia' },
  { codigo: '+90', nombre: 'Turquía' },
  { codigo: '+598', nombre: 'Uruguay' },
  { codigo: '+58', nombre: 'Venezuela' },
];

@Injectable({ providedIn: 'root' })
export class PaisesService {
  getAll(): Pais[] {
    return PAISES;
  }
}
