import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import {
    Stack,
    Heading,
    Box,
    Button,
    Text,
    Group,
    Set,
    Flex,
    FieldStack,
    FieldWrapper,
    useModalState,
    Dialog,
    Card
} from "bumbag"
import { useIsDisplayMenuByRoute } from "../../user/context"
import Disclosure, {
    AddDisclosure,
    EditDisclosure,
    ViewDisclosure
} from "../../dialog/Disclosure"
import {
    useForm,
    useFormContext,
    useFieldArray,
    Controller
} from "react-hook-form"
import FormDialog from "../../form/FormDialog"
import { Trans, useLocale, useTrans } from "../../i18n"
import Backdrop from "../../dialog/Backdrop"
import {
    useAddGameProviderGameList,
    useUpdateGameProviderGame
} from "../../game/games/client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import clone from "clone-deep"
import { Table } from "../../table"
import textColumn from "../../table/column/textColumn"
import { InputField, Input } from "../../input/Input"
import { CheckboxField } from "../../checkbox/Checkbox"
import GameStatus, { GameStatusSelectField } from "../../game/games/GameStatus"
import { GameTypeSelectField } from "../../game/GameType"
import {
    GamePlatformLabel,
    GamePlatformTypeSelectField
} from "../../game/games/GamePlatform"
import { useDropzone } from "react-dropzone"
import { View, Update } from "./menu"
import {
    useAddGameProviderConfig,
    useGameProviderConfig,
    useUpdateGameProviderConfig
} from "../../game/providers/configs/client"
import { SelectField } from "../../select/Select"
import { SwitchField } from "../../switch/Switch"
import { CloseButtonFooter } from "../../form/ConfirmButtonFooter"
import { JsonEditor } from "../../text/editor/JsonEditor"

export const AddGameProviderConfigDisclosure = ({
    gameProviderId,
    onSubmitted
}) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([Update])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <AddDisclosure {...dialog} />
            {dialog.visible && (
                <AddConfigDialog
                    gameProviderId={gameProviderId}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const AddConfigDialog = ({ gameProviderId, onSubmitted, dialog, ...props }) => {
    let title = useTrans({
        zh: `新增游戏商配置`,
        en: `Add Game Provider Config`
    })

    let [formData, setSubmit] = useState()
    let { isLoading, result, error, mutate, deleteCache } =
        useAddGameProviderConfig(formData)

    const onSubmit = (data) => {
        setSubmit({
            gameProviderId,
            ...data
        })
    }

    const reset = () => {
        deleteCache()
        mutate(false, undefined)
        setSubmit(undefined)
    }

    return (
        <Backdrop {...dialog}>
            <FormDialog
                defaultValues={{ status: Active }}
                onSubmit={onSubmit}
                onSubmitted={onSubmitted}
                result={result}
                isLoading={isLoading}
                error={error}
                onCloseErrorAlert={reset}
                dialog={dialog}
                title={title}
                hideBackdrop
                width="500px"
            >
                <FormBody />
            </FormDialog>
        </Backdrop>
    )
}

export const ViewGameProviderConfigDisclosure = ({ id }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([View])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <ViewDisclosure {...dialog} />
            {dialog.visible && <ViewDialog id={id} dialog={dialog} />}
        </>
    )
}

const ViewDialog = ({ id, dialog, ...props }) => {
    let title = useTrans({
        zh: `游戏商配置`,
        en: `Game Provider Config`
    })

    let { isLoading, data, error, mutate, deleteCache } =
        useGameProviderConfig(id)

    if (isLoading) {
        return <LoadingBackdrop {...dialog} />
    }

    if (error) {
        return <AlertDialog {...error} {...dialog} onClickClose={dialog.hide} />
    }

    return (
        <Backdrop {...dialog}>
            <FormDialog
                defaultValues={clone(data)}
                showActionButtons={false}
                footer={<CloseButtonFooter dialog={dialog} alignX="right" />}
                dialog={dialog}
                title={title}
                hideBackdrop
                width="500px"
            >
                <FormBody readOnly />
            </FormDialog>
        </Backdrop>
    )
}

export const EditGameProviderConfigDisclosure = ({ id, onSubmitted }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([Update])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <EditDisclosure {...dialog} />
            {dialog.visible && (
                <EditConfigDialog
                    id={id}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const EditConfigDialog = ({ id, onSubmitted, dialog, ...props }) => {
    let title = useTrans({
        zh: `编辑游戏商配置`,
        en: `Edit Game Provider Config`
    })

    let {
        isLoading: isLoadingGameProviderConfig,
        data,
        error: gameProviderConfigError,
        mutate: mutateGameProviderConfig,
        deleteCache: deleteGameProviderConfigCache
    } = useGameProviderConfig(id)

    let [formData, setSubmit] = useState()
    let { isLoading, result, error, mutate, deleteCache } =
        useUpdateGameProviderConfig(id, formData)

    if (isLoadingGameProviderConfig) {
        return <LoadingBackdrop {...dialog} />
    }

    if (gameProviderConfigError) {
        return (
            <AlertDialog
                {...gameProviderConfigError}
                {...dialog}
                onClickClose={dialog.hide}
            />
        )
    }

    const onSubmit = (data) => {
        setSubmit(data)
    }

    const reset = () => {
        deleteGameProviderConfigCache()
        mutateGameProviderConfig(undefined, false)

        deleteCache()
        mutate(false, undefined)
        setSubmit(undefined)
    }

    return (
        <Backdrop {...dialog}>
            <FormDialog
                defaultValues={clone(data)}
                onSubmit={onSubmit}
                onSubmitted={() => {
                    reset()
                    onSubmitted?.()
                }}
                result={result}
                isLoading={isLoading}
                error={error}
                onCloseErrorAlert={reset}
                dialog={dialog}
                title={title}
                hideBackdrop
                width="500px"
            >
                <FormBody />
            </FormDialog>
        </Backdrop>
    )
}

const FormBody = ({ readOnly = false }) => {
    return (
        <FieldStack>
            <InputField
                name="name"
                required
                readOnly={readOnly}
                autoComplete="off"
                label="Name"
            />
            <InputField
                name="description"
                autoComplete="off"
                readOnly={readOnly}
                label="Description"
            />
            <FieldStack orientation="horizontal">
                <DefaultSwitchField readOnly={readOnly} />
                <ConfigurationStatusSelectField readOnly={readOnly} />
            </FieldStack>
            <InputField
                name="clientId"
                required
                readOnly={readOnly}
                autoComplete="off"
                label="Client Id"
            />
            <InputField
                name="secretKey"
                required
                readOnly={readOnly}
                autoComplete="off"
                label="Secret Key"
            />
            <InputField
                name="desktopEndpoint"
                required
                readOnly={readOnly}
                autoComplete="off"
                label="Desktop Endpoint"
            />
            <InputField
                name="mobileEndpoint"
                required
                readOnly={readOnly}
                autoComplete="off"
                label="Mobile Endpoint"
            />
            <FieldWrapper label="Configuration Data">
                <JsonEditor
                    name="configurationData"
                    disabled={readOnly}
                    height="30vh"
                />
            </FieldWrapper>
        </FieldStack>
    )
}

const DefaultSwitchField = ({ name = "isDefault", disabled, readOnly }) => {
    let label = useTrans({
        zh: "预设",
        en: "Default"
    })

    disabled = disabled || readOnly
    return <SwitchField name={name} disabled={disabled} label={label} />
}

const Active = "Active"
const ActiveTrans = {
    zh: "开启",
    en: "Active"
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
const ConfigurationStatusSelectField = ({
    name = "status",
    disabled,
    readOnly,
    ...props
}) => {
    let label = useTrans({
        zh: "状态",
        en: "Status"
    })
    let placeholder = useTrans({
        zh: "选择状态",
        en: "Select Status"
    })

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

    disabled = disabled || readOnly
    return (
        <SelectField
            name={name}
            label={label}
            placeholder={placeholder}
            options={options}
            disabled={disabled}
            {...props}
        />
    )
}
