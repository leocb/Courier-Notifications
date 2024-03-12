import SoundWhoosh from '../../assets/sounds/whoosh.mp3'
import SoundPop from '../../assets/sounds/pop.mp3'
import SoundYay from '../../assets/sounds/yay.mp3'

export class FxSounds {
  static PlayWhoosh(): void {
    new Audio(SoundWhoosh).play()
  }
  static PlayPop(): void {
    new Audio(SoundPop).play()
  }
  static PlayYay(): void {
    new Audio(SoundYay).play()
  }
}
