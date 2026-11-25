import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { Resume } from './models/resume.model';
import { ResumeService } from './services/resume.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, DatePipe],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  private resumeService = inject(ResumeService);

  resumes: Resume[] = [];
  selectedFile: File | null = null;
  message = '';
  errorMessage = '';
  isLoading = false;
  replacingResumeId: number | null = null;

  ngOnInit(): void {
    this.loadResumes();
  }

  loadResumes(): void {
    this.isLoading = true;
    this.resumeService.getAll().subscribe({
      next: (data) => {
        this.resumes = data;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Unable to load resumes. Check that the backend is running.';
        this.isLoading = false;
      }
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedFile = input.files?.[0] ?? null;
    this.message = '';
    this.errorMessage = '';
  }

  uploadResume(): void {
    if (!this.selectedFile) {
      this.errorMessage = 'Please choose a PDF or DOCX file.';
      return;
    }

    this.resumeService.upload(this.selectedFile).subscribe({
      next: () => {
        this.message = 'Resume uploaded successfully.';
        this.selectedFile = null;
        this.resetFileInput();
        this.loadResumes();
      },
      error: (error) => {
        this.errorMessage = error.error?.message ?? 'Resume upload failed.';
      }
    });
  }

  chooseReplacement(id: number, event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (!file) {
      return;
    }

    this.resumeService.replace(id, file).subscribe({
      next: () => {
        this.message = 'Resume replaced successfully.';
        this.errorMessage = '';
        this.loadResumes();
        input.value = '';
      },
      error: (error) => {
        this.errorMessage = error.error?.message ?? 'Resume replacement failed.';
        input.value = '';
      }
    });
  }

  downloadResume(resume: Resume): void {
    this.resumeService.download(resume.resumeId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const anchor = document.createElement('a');
        anchor.href = url;
        anchor.download = resume.fileName;
        anchor.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => {
        this.errorMessage = 'Unable to download this resume.';
      }
    });
  }

  deleteResume(resume: Resume): void {
    const confirmed = window.confirm(`Delete ${resume.fileName}?`);
    if (!confirmed) {
      return;
    }

    this.resumeService.delete(resume.resumeId).subscribe({
      next: () => {
        this.message = 'Resume deleted successfully.';
        this.errorMessage = '';
        this.loadResumes();
      },
      error: () => {
        this.errorMessage = 'Unable to delete this resume.';
      }
    });
  }

  formatSize(bytes: number): string {
    return `${(bytes / 1024).toFixed(1)} KB`;
  }

  private resetFileInput(): void {
    const input = document.getElementById('resumeFile') as HTMLInputElement | null;
    if (input) {
      input.value = '';
    }
  }
}
