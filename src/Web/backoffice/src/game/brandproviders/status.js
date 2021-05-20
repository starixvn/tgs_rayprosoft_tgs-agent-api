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

const Active = "Active"
const ActiveTrans = {
    zh: "开启",
    en: "Enabled"
}

const Inactive = "Inactive"
const InactiveTrans = {
    zh: "关闭",
    en: Inactive
}

const Maintenance = "Maintenance"
const MaintenanceTrans = {
    zh: "维护",
    en: Maintenance
}

export const Label = ({ value = "" }) => {
    switch (value) {
        case Active:
            return <Trans {...ActiveTrans} />

        case Inactive:
            return <Trans {...InactiveTrans} />

        case Maintenance:
            return <Trans {...MaintenanceTrans} />

        default:
            return value
    }
}

const toPalette = (value) => {
    switch (value) {
        case Active:
            return "success"
        case Inactive:
            return "danger"
        case Maintenance:
            return "warning"
        default:
            return "default"
    }
}

export const StatusMenu = ({ record = {}, onSubmitted, ...props }) => {
    let { id, status } = record
    let [newState, setSubmit] = useState()
    let { result, isLoading, error, deleteCache, mutate } =
        useChangeBrandGameProviderScheduleStatus(id, newState)

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    useEffect(() => {
        if (result && !error) {
            reset()
            onSubmitted?.()
        }
    }, [result, error])

    return (
        <>
            <DropdownMenu
                menu={
                    <>
                        {status != Active && (
                            <DropdownMenu.Item
                                onClick={() => setSubmit(Active)}
                            >
                                <Label value={Active} />
                            </DropdownMenu.Item>
                        )}
                        {status != Inactive && (
                            <DropdownMenu.Item
                                onClick={() => setSubmit(Inactive)}
                            >
                                <Label value={Inactive} />
                            </DropdownMenu.Item>
                        )}
                        {status != Maintenance && (
                            <DropdownMenu.Item
                                onClick={() => setSubmit(Maintenance)}
                            >
                                <Label value={Maintenance} />
                            </DropdownMenu.Item>
                        )}
                    </>
                }
            >
                <Button
                    palette={toPalette(status)}
                    isLoading={isLoading}
                    disabled={isLoading}
                >
                    <Label value={status} />
                </Button>
            </DropdownMenu>
            {error && <Alert {...error} onClickClose={reset} />}
        </>
    )
}

export const StatusMenuField = ({ name, size, ...props }) => {
    let { setValue, control } = useFormContext()
    return (
        <Controller
            name={name}
            control={control}
            shouldUnregister={true}
            render={({
                field: { onChange, onBlur, value, name, ref },
                fieldState: { invalid, isTouched, isDirty }
            }) => (
                <DropdownMenu
                    menu={
                        <>
                            {value != Active && (
                                <DropdownMenu.Item
                                    onClick={() => setValue(name, Active)}
                                >
                                    <Label value={Active} />
                                </DropdownMenu.Item>
                            )}
                            {value != Inactive && (
                                <DropdownMenu.Item
                                    onClick={() => setValue(name, Inactive)}
                                >
                                    <Label value={Inactive} />
                                </DropdownMenu.Item>
                            )}
                            {value != Maintenance && (
                                <DropdownMenu.Item
                                    onClick={() => setValue(name, Maintenance)}
                                >
                                    <Label value={Maintenance} />
                                </DropdownMenu.Item>
                            )}
                        </>
                    }
                >
                    <Button palette={toPalette(value)} size={size} {...props}>
                        <Label value={value} />
                    </Button>
                </DropdownMenu>
            )}
        />
    )
}
