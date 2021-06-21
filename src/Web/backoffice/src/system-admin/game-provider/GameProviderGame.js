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
import { CloseButtonFooter } from "../../form/ConfirmButtonFooter"

export const ViewGameProviderGameDisclosure = ({ record }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([View])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <ViewDisclosure {...dialog} />
            {dialog.visible && <ViewDialog record={record} dialog={dialog} />}
        </>
    )
}

const ViewDialog = ({ record, dialog }) => {
    let title = useTrans({
        zh: "View Game",
        en: "View Game"
    })

    return (
        <Backdrop {...dialog}>
            <FormDialog
                defaultValues={clone(record)}
                title={title}
                hideBackdrop
                title={title}
                dialog={dialog}
                footer={<CloseButtonFooter dialog={dialog} alignX="right" />}
                showActionButtons={false}
                width="60vw"
                css={css`
                    & .bb-DialogContent {
                        position: relative;
                    }
                `}
            >
                <GameProviderGameFormBody readOnly />
            </FormDialog>
        </Backdrop>
    )
}

export const EditGameProviderGameDisclosure = ({ record, onSubmitted }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([Update])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <EditDisclosure {...dialog} />
            {dialog.visible && (
                <EditGameDialog
                    record={record}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

export const AddGameProviderGameDisclosure = ({
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
                <AddGameDialog
                    gameProviderId={gameProviderId}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const AddGameDialog = ({ gameProviderId, onSubmitted, dialog }) => {
    let title = useTrans({
        zh: "Add Game",
        en: "Add Game"
    })
    let [formData, setSubmit] = useState()

    let { isLoading, result, error, mutate, deleteCache } =
        useAddGameProviderGameList(formData)

    const onSubmit = (data) => {
        setSubmit({
            gameProviderId,
            games: [data]
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
                defaultValues={{ status: GameStatus.Active }}
                title={title}
                onSubmit={onSubmit}
                onSubmitted={onSubmitted}
                result={result}
                isLoading={isLoading}
                error={error}
                onCloseErrorAlert={reset}
                stopPropagation
                hideBackdrop
                title={title}
                dialog={dialog}
                width="60vw"
                css={css`
                    & .bb-DialogContent {
                        position: relative;
                    }
                `}
            >
                <GameProviderGameFormBody />
            </FormDialog>
        </Backdrop>
    )
}

const EditGameDialog = ({ record, onSubmitted, dialog }) => {
    let title = useTrans({
        zh: "Edit Game",
        en: "Edit Game"
    })
    let [formData, setSubmit] = useState()

    let { isLoading, result, error, mutate, deleteCache } =
        useUpdateGameProviderGame(record?.id, formData)

    const onSubmit = (data) => {
        setSubmit(data)
    }

    const reset = () => {
        deleteCache()
        mutate(false, undefined)
        setSubmit(undefined)
    }
    return (
        <Backdrop {...dialog}>
            <FormDialog
                defaultValues={clone(record)}
                title={title}
                onSubmit={onSubmit}
                onSubmitted={onSubmitted}
                result={result}
                isLoading={isLoading}
                error={error}
                onCloseErrorAlert={reset}
                stopPropagation
                hideBackdrop
                title={title}
                dialog={dialog}
                width="60vw"
                css={css`
                    & .bb-DialogContent {
                        position: relative;
                    }
                `}
            >
                <GameProviderGameFormBody />
            </FormDialog>
        </Backdrop>
    )
}

export const GameProviderGameFormBody = ({ readOnly = false }) => {
    let { watch, getValues } = useFormContext()
    let id = getValues("id")

    let isCreate = !id
    let isEdit = !isCreate
    let disabled = readOnly
    return (
        <FieldStack>
            <FieldStack orientation="horizontal">
                {isEdit && (
                    <InputField
                        name="id"
                        value={watch("id")}
                        readOnly
                        label="Id"
                    />
                )}
                <InputField
                    name="externalId"
                    required
                    readOnly={readOnly}
                    label="External Id"
                />
            </FieldStack>
            <FieldStack orientation="horizontal">
                <InputField
                    name="code"
                    required
                    readOnly={readOnly}
                    label="Code"
                />
                <InputField
                    name="name"
                    required
                    readOnly={readOnly}
                    label="Name"
                />
            </FieldStack>
            <InputField
                name="description"
                readOnly={readOnly}
                label="Description"
            />
            <FieldStack orientation="horizontal">
                <GameStatusSelectField
                    name="status"
                    required
                    disabled={disabled}
                    label="Status"
                />
                <GameTypeSelectField
                    name="type"
                    required
                    disabled={disabled}
                    label="Type"
                />
                <GamePlatformTypeSelectField
                    name="platformType"
                    required
                    disabled={disabled}
                    label="Platform Type"
                />
            </FieldStack>
            <InputField
                name="endpointPath"
                readOnly={readOnly}
                label="Endpoint Path"
            />
            <InputField name="ruleUrl" readOnly={readOnly} label="Rule Url" />
            <FieldStack orientation="horizontal">
                <CheckboxField
                    name="subGame"
                    disabled={disabled}
                    label="Sub Game"
                />
                <CheckboxField
                    name="availableToBrand"
                    disabled={disabled}
                    label="Available To Brand"
                />
            </FieldStack>
            <FieldStack orientation="horizontal">
                <CheckboxField
                    name="hasTurnoverOnTieBet"
                    disabled={disabled}
                    label="Turnover On Tie Bet"
                />
                <CheckboxField name="html5" disabled={disabled} label="Html5" />
            </FieldStack>
            <InputField
                name="availableCurrencies"
                readOnly={readOnly}
                label="Available Currencies"
            />
        </FieldStack>
    )
}
