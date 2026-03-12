export interface ApiResponse<T> {
  cod_retorno: number;
  mensagem: string | null;
  data: T;
}