export const Desktop = "Desktop"
const DesktopValue = 0

export const Mobile = "Mobile"
const MobileValue = 1

export const toValue = (type = "") => {
  switch (type) {
    case Desktop:
      return DesktopValue
    case Mobile:
      return MobileValue
  }
}

export const platformList = () => [Desktop, Mobile]
