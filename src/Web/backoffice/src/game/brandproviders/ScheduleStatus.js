import React, { useState, useEffect, useMemo } from "react"
import {
    css,
    Stack,
    Heading,
    Box,
    Button,
    Select as _Select,
    SelectMenu,
    DropdownMenu
} from "bumbag"
import { useForm, useFormContext, Controller } from "react-hook-form"
import { useChangeBrandGameProviderScheduleStatus } from "./client"
import Alert from "../../dialog/Alert"
import { useTrans, Trans } from "../../i18n"

const Enabled = "Enabled"
const EnabledTrans = {
    zh: "开启",
    en: "Enabled"
}

const Disabled = "Disabled"
const DisabledTrans = {
    zh: "关闭",
    en: Disabled
}

export const ScheduleStatus = {
    Enabled,
    Disabled
}

export const ScheduleStatusLabel = ({ value = "" }) => {
    switch (value) {
        case Enabled:
            return <Trans {...EnabledTrans} />

        case Disabled:
            return <Trans {...DisabledTrans} />

        default:
            return value
    }
}

const toPalette = (value) => {
    switch (value) {
        case Enabled:
            return "success"
        case Disabled:
            return "danger"
        default:
            return "default"
    }
}

export const MaintenanceStatusToggleButton = ({
    name = "",
    value,
    ...props
}) => {
    let { control, setValue } = useFormContext()
    return (
        <Controller
            control={control}
            name={name}
            render={({
                field: { onChange, onBlur, value, name, ref },
                fieldState: { invalid, isTouched, isDirty }
            }) => {
                return (
                    <Button
                        palette={toPalette(value)}
                        onClick={(event) => {
                            let status = value == Enabled ? Disabled : Enabled
                            setValue(name, status)
                        }}
                    >
                        <ScheduleStatusLabel value={value} />
                    </Button>
                )
            }}
        />
    )
}
