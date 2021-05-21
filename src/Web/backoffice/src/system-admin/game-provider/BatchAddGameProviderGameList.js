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
    Card,
    Spinner,
    Alert
} from "bumbag"
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
    useGameProvider,
    useUpdateGameProvider,
    useUploadGameProviderGames,
    useAddGameProviderGameList
} from "../../game/providers/client"
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
import { GameProviderGameDialog } from "./GameProviderGame"

export const BatchAddGameProviderGameListDisclosure = ({
    gameProviderId,
    onSubmitted
}) => {
    let dialog = useModalState()

    return (
        <>
            <Disclosure {...dialog}>
                <Trans zh="批量" en="Batch" />
            </Disclosure>
            {dialog.visible && (
                <BatchAddGameListDialog
                    gameProviderId={gameProviderId}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const BatchAddGameListDialog = ({ gameProviderId, onSubmitted, dialog }) => {
    let title = useTrans({
        zh: "批量新增游戏",
        en: "Batch Create Games"
    })

    let [games, setGames] = useState([])
    let { isLoading, result, error, mutate, deleteCache } =
        useAddGameProviderGameList(gameProviderId, games)

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setGames([])
    }

    let uploadRequired = !games || games.length == 0
    return (
        <Backdrop {...dialog}>
            <FormDialog
                title={title}
                showActionButtons={false}
                isLoading={isLoading}
                result={result}
                error={error}
                onCloseErrorAlert={reset}
                onSubmitted={onSubmitted}
                dialog={dialog}
                hideBackdrop
                width="60vw"
            >
                {uploadRequired && (
                    <UploadGameListCard
                        gameProviderId={gameProviderId}
                        onChange={(uploadedGames) => {
                            setGames(uploadedGames)
                        }}
                    />
                )}
                {JSON.stringify(games)}
            </FormDialog>
        </Backdrop>
    )
}

const UploadGameListCard = ({ gameProviderId, onChange }) => {
    let [formData, setSubmit] = useState()
    const {
        acceptedFiles,
        getRootProps,
        getInputProps,
        isDragActive,
        isDragAccept,
        isDragReject
    } = useDropzone({
        maxFiles: 1,
        accept: ".xlsx",
        onDrop: (files) => {
            if (files.length > 0) {
                let [file] = files
                let formData = new FormData()
                formData.append("id", gameProviderId)
                formData.append("file", file, file.name)
                setSubmit(formData)
            }
        }
    })

    let { isLoading, result, error, deleteCache, mutate } =
        useUploadGameProviderGames(formData)

    useEffect(() => {
        let games = result?.games?.map((game) => ({
            ...clone(game),
            status: GameStatus.Active
        }))
        onChange?.(games)
    }, [result])

    useEffect(() => {
        if (error) {
            //TODO: remain error msg?
            reset()
        }
    }, [error])

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    let notSelectFile = !formData?.file
    return (
        <>
            {notSelectFile && (
                <Card
                    {...getRootProps({
                        refKey: "elementRef "
                    })}
                >
                    <input {...getInputProps()} />
                    <p>
                        <Trans zh="选择档案" en="Select File" />
                    </p>
                    {error && (
                        <Alert type="danger">{JSON.stringify(error)}</Alert>
                    )}
                </Card>
            )}
            {isLoading && (
                <Card>
                    <Spinner />
                </Card>
            )}
        </>
    )
}

export const GameProviderBatchGameDisclosure = ({ record, onChange }) => {
    let dialog = useModalState()

    return (
        <>
            <EditDisclosure {...dialog} />
            {dialog.visible && (
                <GameProviderGameDialog
                    record={record}
                    onChange={onChange}
                    dialog={dialog}
                />
            )}
        </>
    )
}
