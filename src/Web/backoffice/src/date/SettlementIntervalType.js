import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Select } from "../select/Select"
import { useTrans } from "../i18n"

const Daily = "Daily"
const DailyTrans = {
    zh: "每日",
    en: "Daily"
}

const Weekly = "Weekly"
const WeeklyTrans = {
    zh: "每周",
    en: "Weekly"
}

const Monthly = "Monthly"
const MonthlyTrans = {
    zh: "每月",
    en: "Monthly"
}

const Instantly = "Instantly"

export const SettlementIntervalTypeSelect = ({
    placeholder,
    placeholderDisabled = false,
    ...props
}) => {
    let defaultPlaceholder = useTrans({
        zh: "周期",
        en: "Interval"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let options = [
        {
            label: useTrans(DailyTrans),
            value: Daily
        },
        {
            label: useTrans(WeeklyTrans),
            value: Weekly
        },
        {
            label: useTrans(MonthlyTrans),
            value: Monthly
        }
    ]

    return <Select placeholder={placeholder} options={options} {...props} />
}

export default {
    Daily,
    Weekly,
    Monthly,
    Instantly
}
