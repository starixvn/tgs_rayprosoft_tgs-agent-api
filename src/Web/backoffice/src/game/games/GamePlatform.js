import React, { useState, useEffect, useMemo } from "react"
import { useTrans, Trans } from "../../i18n"

const Desktop = "Desktop"
const DesktopTrans = {
    zh: "PC",
    en: "PC"
}

const Mobile = "Mobile"
const MobileTrans = {
    zh: "M",
    en: "M"
}

const DesktopAndMobile = "DesktopAndMobile"
const DesktopAndMobileTrans = {
    zh: "PC/M",
    en: "PC/M"
}

export const GamePlatformLabel = ({ value }) => {
    switch (value) {
        case Desktop:
            return <Trans {...DesktopTrans} />

        case Mobile:
            return <Trans {...MobileTrans} />

        case DesktopAndMobile:
            return <Trans {...DesktopAndMobileTrans} />

        default:
            return value ?? null
    }
}
