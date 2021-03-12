import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import {
    Table,
    Set,
    Button,
    Box,
    Stack,
    PageWithHeader,
    TopNav,
    Pagination
} from "bumbag"
import {
    Switch,
    Route,
    Link,
    useHistory,
    useParams,
    useRouteMatch
} from "react-router-dom"
import BrandHeader from "../Header"
import PlayerPage from "./Page"
import { LoginDialogButton } from "./LoginDialog"
import { useBrandPlayerList } from "../../client/brand/player"

const DefaultRows = 20
export const PlayerList = ({ brandToken }) => {
    let params = useParams()
    let { brandCode, currency, playerType } = params
    let { path, url } = useRouteMatch()

    let [state, setState] = useState(params)
    let { result, error, isLoading, mutate, ...rest } = useBrandPlayerList(
        state
    )

    let currentPage = Math.ceil((result?.totalCount ?? 0) / DefaultRows)
    let players = result?.items ?? []

    useEffect(() => {
        setState({
            ...params,
            page: 1,
            rows: DefaultRows
        })
    }, [params])

    return (
        <Switch>
            <Route exact path={path}>
                <PageWithHeader
                    header={<BrandHeader onPlayerCreated={mutate} />}
                >
                    <Stack padding="major-1">
                        <PlayerTable
                            brandCode={brandCode}
                            brandToken={brandToken}
                            currency={currency}
                            players={players}
                            currentPage={currentPage}
                            onChangePage={(page) =>
                                setState({ ...params, page, rows: DefaultRows })
                            }
                        />
                    </Stack>
                </PageWithHeader>
            </Route>
            <Route path={`${path}/:username`}>
                <PlayerPage brandToken={brandToken} />
            </Route>
        </Switch>
    )
}

const PlayerTable = ({
    brandCode,
    currency,
    brandToken,
    players,
    currentPage,
    onChangePage
}) => {
    return (
        <Set orientation="vertical">
            <Table width="100%" overflow="auto" isHoverable>
                <Table.Head>
                    <Table.Row>
                        <Table.HeadCell>Username</Table.HeadCell>
                        <Table.HeadCell>Player Code</Table.HeadCell>
                        <Table.HeadCell>Player Status</Table.HeadCell>
                        <Table.HeadCell>Actions</Table.HeadCell>
                    </Table.Row>
                </Table.Head>
                <Table.Body>
                    {players.map((player, index) => (
                        <PlayerRow
                            key={index}
                            brandCode={brandCode}
                            currency={currency}
                            brandToken={brandToken}
                            player={player}
                        />
                    ))}
                </Table.Body>
            </Table>
            <Pagination
                currentPage={currentPage}
                onChangePage={onChangePage}
                numberOfPages={DefaultRows}
            />
        </Set>
    )
}

const PlayerRow = ({ player, brandCode, currency, brandToken }) => {
    return (
        <Table.Row>
            <Table.Cell>{player.username}</Table.Cell>
            <Table.Cell>{player.playerCode}</Table.Cell>
            <Table.Cell>{player.status}</Table.Cell>
            <Table.Cell>
                <LoginDialogButton
                    brandCode={brandCode}
                    currency={currency}
                    brandToken={brandToken}
                    username={player.username}
                />
            </Table.Cell>
        </Table.Row>
    )
}

export default PlayerTable
