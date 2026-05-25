// Pomodoro Timer (Bonus)

class PomodoroTimer {
    constructor() {
        this.workTime = 25 * 60; // 25 minutes
        this.breakTime = 5 * 60;  // 5 minutes
        this.longBreakTime = 15 * 60; // 15 minutes
        this.currentTime = this.workTime;
        this.isRunning = false;
        this.isWork = true;
        this.cycles = 0;
        this.timer = null;
    }

    start() {
        if (this.isRunning) return;
        this.isRunning = true;
        this.timer = setInterval(() => this.tick(), 1000);
    }

    pause() {
        this.isRunning = false;
        clearInterval(this.timer);
    }

    reset() {
        this.pause();
        this.currentTime = this.workTime;
        this.isWork = true;
        this.cycles = 0;
        this.updateDisplay();
    }

    tick() {
        this.currentTime--;
        if (this.currentTime <= 0) {
            this.complete();
        }
        this.updateDisplay();
    }

    complete() {
        this.pause();
        // Play notification sound
        this.playSound();

        if (this.isWork) {
            this.cycles++;
            this.isWork = false;
            this.currentTime = this.cycles % 4 === 0 ? this.longBreakTime : this.breakTime;
        } else {
            this.isWork = true;
            this.currentTime = this.workTime;
        }

        // Show notification
        if ('Notification' in window && Notification.permission === 'granted') {
            new Notification('Pomodoro', {
                body: this.isWork ? 'Pause terminee ! Retour au travail.' : 'Travail termine ! Prenez une pause.',
                icon: '/images/logo.png'
            });
        }
    }

    playSound() {
        const audio = new Audio('/sounds/notification.mp3');
        audio.play().catch(() => {});
    }

    updateDisplay() {
        const minutes = Math.floor(this.currentTime / 60);
        const seconds = this.currentTime % 60;
        const display = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;

        const timerElement = document.getElementById('pomodoroTimer');
        if (timerElement) {
            timerElement.textContent = display;
        }

        const statusElement = document.getElementById('pomodoroStatus');
        if (statusElement) {
            statusElement.textContent = this.isWork ? 'Travail' : 'Pause';
            statusElement.className = `badge ${this.isWork ? 'bg-danger' : 'bg-success'}`;
        }
    }

    getProgress() {
        const total = this.isWork ? this.workTime : (this.cycles % 4 === 0 ? this.longBreakTime : this.breakTime);
        return ((total - this.currentTime) / total) * 100;
    }
}

// Initialize Pomodoro
let pomodoro = null;

document.addEventListener('DOMContentLoaded', function() {
    if (document.getElementById('pomodoroTimer')) {
        pomodoro = new PomodoroTimer();
        pomodoro.updateDisplay();

        document.getElementById('pomodoroStart')?.addEventListener('click', () => pomodoro?.start());
        document.getElementById('pomodoroPause')?.addEventListener('click', () => pomodoro?.pause());
        document.getElementById('pomodoroReset')?.addEventListener('click', () => pomodoro?.reset());
    }
});