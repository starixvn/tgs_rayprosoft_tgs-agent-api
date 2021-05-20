import React, { useState, useEffect, useMemo } from "react"
import { useTrans, Trans } from "../../i18n"
import { SelectField } from "../../select/Select"

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

export const GamePlatformTypeSelectField = ({ placeholder, ...props }) => {
    let defaultPlaceholder = useTrans({
        zh: "支持设备",
        en: "Platform"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let options = [
        {
            label: useTrans(DesktopTrans),
            value: Desktop
        },
        {
            label: useTrans(MobileTrans),
            value: Mobile
        },
        {
            label: useTrans(DesktopAndMobileTrans),
            value: DesktopAndMobile
        }
    ]

    return (
        <SelectField placeholder={placeholder} options={options} {...props} />
    )
}
