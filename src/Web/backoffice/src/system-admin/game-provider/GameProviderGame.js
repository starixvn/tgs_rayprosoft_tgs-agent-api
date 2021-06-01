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
    EditDisclosure
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
import { Update } from "./menu"

//TODO: edit & view disclosure
export const GameProviderGameDisclosure = ({ record, onSubmitted }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([Update])
    if (!isDisplay) {
        return null
    }

    let isCreate = !record?.id
    let isEdit = !isCreate
    return (
        <>
            {isCreate && <AddDisclosure {...dialog} />}
            {isEdit && <EditDisclosure {...dialog} />}
            {dialog.visible && (
                <GameProviderGameDialog
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

const GameProviderGameDialog = ({ record, onSubmitted, dialog }) => {
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

//TODO: required InputField not work?
export const GameProviderGameFormBody = ({}) => {
    let { watch, getValues } = useFormContext()
    let id = getValues("id")

    let isCreate = !id
    let isEdit = !isCreate
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
                <InputField name="externalId" required label="External Id" />
            </FieldStack>
            <FieldStack orientation="horizontal">
                <InputField name="code" required label="Code" />
                <InputField name="name" required label="Name" />
            </FieldStack>
            <InputField name="description" label="Description" />
            <FieldStack orientation="horizontal">
                <GameStatusSelectField name="status" required label="Status" />
                <GameTypeSelectField name="type" required label="Type" />
                <GamePlatformTypeSelectField
                    name="platformType"
                    required
                    label="Platform Type"
                />
            </FieldStack>
            <InputField name="endpointPath" label="Endpoint Path" />
            <InputField name="ruleUrl" label="Rule Url" />
            <FieldStack orientation="horizontal">
                <CheckboxField name="subGame" label="Sub Game" />
                <CheckboxField
                    name="availableToBrand"
                    label="Available To Brand"
                />
            </FieldStack>
            <FieldStack orientation="horizontal">
                <CheckboxField
                    name="hasTurnoverOnTieBet"
                    label="Turnover On Tie Bet"
                />
                <CheckboxField name="html5" label="Html5" />
            </FieldStack>
            <InputField
                name="availableCurrencies"
                label="Available Currencies"
            />
        </FieldStack>
    )
}
