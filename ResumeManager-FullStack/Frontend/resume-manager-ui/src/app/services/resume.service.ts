import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Resume } from '../models/resume.model';

@Injectable({
  providedIn: 'root'
})
export class ResumeService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7043/api/resumes';

  getAll(): Observable<Resume[]> {
    return this.http.get<Resume[]>(this.apiUrl);
  }

  upload(file: File): Observable<Resume> {
    const formData = new FormData();
    formData.append('File', file);
    return this.http.post<Resume>(this.apiUrl, formData);
  }

  replace(id: number, file: File): Observable<Resume> {
    const formData = new FormData();
    formData.append('File', file);
    return this.http.put<Resume>(`${this.apiUrl}/${id}`, formData);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  download(id: number): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${id}/download`, {
      responseType: 'blob'
    });
  }
}
