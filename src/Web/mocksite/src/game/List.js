import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import {
    Table,
    Set,
    Button,
    Box,
    Stack,
    PageWithHeader,
    TopNav,
    Modal,
    Dialog,
    Text
} from "bumbag"
import {
    Switch,
    Route,
    Link,
    useHistory,
    useParams,
    useRouteMatch,
    useLocation
} from "react-router-dom"
import { useGameList } from "../client/provider"
import { useGameUrl } from "../client/provider/game"
import { toValue } from "../platform"

export const GameList = ({
    brandCode,
    currency,
    username,
    gameProviderCode,
    platform,
    language
}) => {
    let platformtype = toValue(platform)
    let { items: games = [], ...rest } = useGameList(
        { brandCode, username, currency, platformtype },
        { gameProviderCode, language }
    )

    return useMemo(
        () => (
            <Set orientation="vertical">
                <Table width="100%" overflow="auto" isHoverable>
                    <Table.Head>
                        <Table.Row>
                            <Table.HeadCell>Name</Table.HeadCell>
                            <Table.HeadCell>Status</Table.HeadCell>
                            <Table.HeadCell>Actions</Table.HeadCell>
                        </Table.Row>
                    </Table.Head>
                    <Table.Body>
                        {games.map((game, index) => (
                            <GameRow
                                key={index}
                                brandCode={brandCode}
                                currency={currency}
                                username={username}
                                gameProviderCode={gameProviderCode}
                                game={game}
                                platform={platform}
                                language={language}
                            />
                        ))}
                    </Table.Body>
                </Table>
            </Set>
        ),
        [JSON.stringify(games)]
    )
}

const GameRow = ({
    brandCode,
    currency,
    username,
    gameProviderCode,
    platform,
    language,
    game
}) => {
    return (
        <Table.Row>
            <Table.Cell>{game.name}</Table.Cell>
            <Table.Cell>{game.status}</Table.Cell>
            <Table.Cell>
                <LaunchGameButton
                    game={game}
                    brandCode={brandCode}
                    currency={currency}
                    username={username}
                    gameProviderCode={gameProviderCode}
                    platform={platform}
                    language={language}
                />
            </Table.Cell>
        </Table.Row>
    )
}

const LaunchGameButton = ({
    game,
    brandCode,
    currency,
    username,
    gameProviderCode,
    platform,
    language
}) => {
    let platformtype = toValue(platform)
    let [gameWindow, setGameWin] = useState()
    let [formData, setSubmit] = useState()
    let { url, error, mutate } = useGameUrl(
        { brandCode, username, currency, platformtype },
        formData
    )
    let [errorMessage, setErrorMessage] = useState()
    const errorDialog = Modal.useState()

    const reset = () => {
        setGameWin(undefined)
        setSubmit(undefined)
        mutate(undefined, false)
    }

    useEffect(() => {
        if (url && gameWindow) {
            gameWindow.location = url
            reset()
        }
    }, [url])

    useEffect(() => {
        if (error) {
            setErrorMessage(JSON.stringify(error))
            errorDialog.show()
        }
        if (error && gameWindow) {
            gameWindow.close()
            reset()
        }
    }, [gameWindow, error])
    return (
        <>
            <Button
                onClick={(event) => {
                    setSubmit({
                        gameProviderCode: gameProviderCode,
                        gameCode: game?.code,
                        gameId: game?.id,
                        language
                    })

                    let win = window.open()
                    setGameWin(win)
                }}
            >
                Launch
            </Button>
            <Dialog.Modal
                title="Error"
                showCloseButton
                type="warning"
                onClickClose={() => {
                    setErrorMessage("")
                }}
                {...errorDialog}
            >
                <Text.Block>{errorMessage}</Text.Block>
            </Dialog.Modal>
        </>
    )
}
