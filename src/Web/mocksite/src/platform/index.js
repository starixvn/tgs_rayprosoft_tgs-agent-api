export const Desktop = "Desktop"
const DesktopValue = 0

export const Mobile = "Mobile"
const MobileValue = 1

export const Type = {
    Desktop: DesktopValue,
    Mobile: MobileValue
}

export const toValue = (type = "") => {
    switch (type) {
        case Desktop:
            return DesktopValue
        case Mobile:
            return MobileValue
    }
}

export const platformOf = (value) => {
    switch (value) {
        case DesktopValue:
            return Desktop
        case MobileValue:
            return Mobile
        default:
            return Desktop
    }
}

export const platformList = () => [Desktop, Mobile]

export const platformOptions = () =>
    platformList().map((platform) => ({
        label: platform,
        value: toValue(platform)
    }))
