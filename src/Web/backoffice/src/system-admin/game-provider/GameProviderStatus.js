import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { Stack, Heading, Box, Button, DropdownMenu } from "bumbag"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute
} from "../../user/context"
import { useForm, useFormContext, Controller } from "react-hook-form"
import { useChangeGameProviderStatus } from "../../game/providers/client"
import Alert from "../../dialog/Alert"
import { useTrans, Trans } from "../../i18n"
import { Update } from "./menu"

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

export default {
    Active,
    Inactive,
    Maintenance
}

export const GameProviderStatusLabel = ({ value = "" }) => {
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

export const GameProviderStatusMenu = ({
    record = {},
    onSubmitted,
    ...props
}) => {
    let hasUpdateAuth = useIsDisplayMenuByRoute([Update])
    let { id, status } = record
    let [newState, setSubmit] = useState()
    let { result, isLoading, error, deleteCache, mutate } =
        useChangeGameProviderStatus(id, newState)

    useEffect(() => {
        if (result && !error) {
            reset()
            onSubmitted?.()
        }
    }, [result, error])

    if (!hasUpdateAuth) {
        return (
            <Button palette={toPalette(status)} disabled isStatic>
                <GameProviderStatusLabel value={status} />
            </Button>
        )
    }

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    return (
        <>
            <DropdownMenu
                menu={
                    <>
                        {status != Active && (
                            <DropdownMenu.Item
                                onClick={() => setSubmit(Active)}
                            >
                                <GameProviderStatusLabel value={Active} />
                            </DropdownMenu.Item>
                        )}
                        {status != Inactive && (
                            <DropdownMenu.Item
                                onClick={() => setSubmit(Inactive)}
                            >
                                <GameProviderStatusLabel value={Inactive} />
                            </DropdownMenu.Item>
                        )}
                        {status != Maintenance && (
                            <DropdownMenu.Item
                                onClick={() => setSubmit(Maintenance)}
                            >
                                <GameProviderStatusLabel value={Maintenance} />
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
                    <GameProviderStatusLabel value={status} />
                </Button>
            </DropdownMenu>
            {error && <Alert {...error} onClickClose={reset} />}
        </>
    )
}

export const GameProviderStatusSelect = ({ name = "status", ...props }) => {
    /*
  let options = [
    {
      label: useTrans(ActiveTrans),
      value: Active
    },
    {
      label: useTrans(InactiveTrans),
      value: Inactive
    },
    {
      label: useTrans(MaintenanceTrans),
      value: Maintenance
    }
  ]

  let { watch } = useFormContext()
  let value = watch(name)
  let state = toPalette(value)

  return <Select name={name} options={options} state={state} {...props} />
  */

    let { control, setValue, watch } = useFormContext()
    let defaultValue = watch(name)

    return (
        <Controller
            name={name}
            control={control}
            shouldUnregister={true}
            defaultValue={watch(name)}
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
                                    <GameProviderStatusLabel value={Active} />
                                </DropdownMenu.Item>
                            )}
                            {value != Inactive && (
                                <DropdownMenu.Item
                                    onClick={() => setValue(name, Inactive)}
                                >
                                    <GameProviderStatusLabel value={Inactive} />
                                </DropdownMenu.Item>
                            )}
                            {value != Maintenance && (
                                <DropdownMenu.Item
                                    onClick={() => setValue(name, Maintenance)}
                                >
                                    <GameProviderStatusLabel
                                        value={Maintenance}
                                    />
                                </DropdownMenu.Item>
                            )}
                        </>
                    }
                >
                    <Button palette={toPalette(value)} {...props}>
                        <GameProviderStatusLabel value={value} />
                    </Button>
                </DropdownMenu>
            )}
        />
    )
}
