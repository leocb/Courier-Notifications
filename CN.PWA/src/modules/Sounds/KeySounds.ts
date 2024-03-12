import SoundKeyDown1 from '../../assets/sounds/key/KeyDown1.mp3'
import SoundKeyDown2 from '../../assets/sounds/key/KeyDown2.mp3'
import SoundKeyDown3 from '../../assets/sounds/key/KeyDown3.mp3'
import SoundKeyDown4 from '../../assets/sounds/key/KeyDown4.mp3'
import SoundKeyDown5 from '../../assets/sounds/key/KeyDown5.mp3'
import SoundKeyUp1 from '../../assets/sounds/key/KeyUp1.mp3'
import SoundKeyUp2 from '../../assets/sounds/key/KeyUp2.mp3'
import SoundKeyUp3 from '../../assets/sounds/key/KeyUp3.mp3'
import SoundKeyUp4 from '../../assets/sounds/key/KeyUp4.mp3'
import SoundKeyUp5 from '../../assets/sounds/key/KeyUp5.mp3'

export class KeySounds {
  static AllKeyDown = [SoundKeyDown1, SoundKeyDown2, SoundKeyDown3, SoundKeyDown4, SoundKeyDown5]
  static AllKeyUp = [SoundKeyUp1, SoundKeyUp2, SoundKeyUp3, SoundKeyUp4, SoundKeyUp5]

  static PlayRandomKeyDown(): void {
    new Audio(KeySounds.AllKeyDown[(KeySounds.AllKeyDown.length * Math.random()) | 0]).play()
  }
  static PlayRandomKeyUp(): void {
    new Audio(KeySounds.AllKeyUp[(KeySounds.AllKeyUp.length * Math.random()) | 0]).play()
  }
}
