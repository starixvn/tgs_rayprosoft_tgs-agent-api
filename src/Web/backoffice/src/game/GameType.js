import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Locales, Trans, useLocale, useTrans } from "../i18n"
import { Select } from "../select/Select"
import { useGameTypeGroupList } from "./client"
import { mapOmit } from "../list/map"

const Sportsbook = "Sportsbook"
const SportsbookTrans = {
    zh: "体育",
    en: "Sportsbook"
}

const VirtualSports = "VirtualSports"
const VirtualSportsTrans = {
    zh: "虚拟体育",
    en: "Virtual Sports"
}

const VirtualSportsLobby = "VirtualSportsLobby"
const VirtualSportsLobbyTrans = {
    zh: "虚拟体育大厅",
    en: "Virtual Sports Lobby"
}

const ESports = "ESports"
const ESportsTrans = {
    zh: "电竞",
    en: "E-sports"
}

const LiveDealer = "LiveDealer"
const LiveDealerTrans = {
    zh: "真人",
    en: "Live Dealer"
}

const LiveDealerLobby = "LiveDealerLobby"
const LiveDealerLobbyTrans = {
    zh: "真人大厅",
    en: "Live Dealer Lobby"
}

const CardGame = "CardGame"
const CardGameTrans = {
    zh: "棋牌",
    en: "Card Game"
}

const CardGameLobby = "CardGameLobby"
const CardGameLobbyTrans = {
    zh: "棋牌大厅",
    en: "Card Game Lobby"
}

const Fishing = "Fishing"
const FishingTrans = {
    zh: "捕鱼",
    en: "Fishing"
}

const FishingLobby = "FishingLobby"
const FishingLobbyTrans = {
    zh: "捕鱼大厅",
    en: "Fishing Lobby"
}

const Arcade = "Arcade"
const ArcadeTrans = {
    zh: "街机",
    en: "Arcade Game"
}

const ArcadeLobby = "ArcadeLobby"
const ArcadeLobbyTrans = {
    zh: "街机大厅",
    en: "Arcade Game Lobby"
}

const Slot = "Slot"
const SlotTrans = {
    zh: "老虎机",
    en: "Slot Game"
}

const Lottery = "Lottery"
const LotteryTrans = {
    zh: "彩票",
    en: "Lottery"
}

const LotteryLobby = "LotteryLobby"
const LotteryLobbyTrans = {
    zh: "彩票大厅",
    en: "Lottery Lobby"
}

const TableGame = "TableGame"
const TableGameTrans = {
    zh: "桌面游戏",
    en: "Table Game"
}

export const GameType = {
    Sportsbook,
    VirtualSports,
    VirtualSportsLobby,
    ESports,
    LiveDealer,
    LiveDealerLobby,
    CardGame,
    CardGameLobby,
    Fishing,
    FishingLobby,
    Arcade,
    ArcadeLobby,
    Lottery,
    LotteryLobby,
    Slot,
    TableGame
}

//TODO
export const SportGameTypes = [Sportsbook, VirtualSports, ESports]

//TODO
export const isSportGameType = (value = "") => SportGameTypes.includes(value)

const LiveDealerGroupTrans = {
    zh: "真人",
    en: "Live Dealer"
}

const SportsGroupTrans = {
    zh: "体育",
    en: "Sports"
}

const EGameGroupTrans = {
    zh: "电子",
    en: "EGame"
}

const CardGameGroupTrans = {
    zh: "棋牌",
    en: "Card Game"
}

const VirtualSportsGroupTrans = {
    zh: "虚拟体育",
    en: "Virtual Sports"
}

const LotteryGroupTrans = {
    zh: "彩票",
    en: "彩票"
}

const AllTrans = [
    {
        name: "LiveDealer",
        trans: LiveDealerGroupTrans,
        items: [
            {
                name: LiveDealer,
                trans: LiveDealerTrans
            },
            {
                name: LiveDealerLobby,
                trans: LiveDealerLobbyTrans
            }
        ]
    },
    {
        name: "Sports",
        trans: SportsGroupTrans,
        items: [
            {
                name: Sportsbook,
                trans: SportsbookTrans
            },
            {
                name: ESports,
                trans: ESportsTrans
            }
        ]
    },
    {
        name: "EGame",
        trans: EGameGroupTrans,
        items: [
            {
                name: Slot,
                trans: SlotTrans
            },
            {
                name: Fishing,
                trans: FishingTrans
            },
            {
                name: FishingLobby,
                trans: FishingLobbyTrans
            },
            {
                name: Arcade,
                trans: ArcadeTrans
            },
            {
                name: ArcadeLobby,
                trans: ArcadeLobbyTrans
            },
            {
                name: TableGame,
                trans: TableGameTrans
            }
        ]
    },
    {
        name: "CardGame",
        trans: CardGameGroupTrans,
        items: [
            {
                name: CardGame,
                trans: CardGameTrans
            },
            {
                name: CardGameLobby,
                trans: CardGameLobbyTrans
            }
        ]
    },
    {
        name: "VirtualSports",
        trans: VirtualSportsGroupTrans,
        items: [
            {
                name: VirtualSports,
                trans: VirtualSportsTrans
            },
            {
                name: VirtualSportsLobby,
                trans: VirtualSportsLobbyTrans
            }
        ]
    },
    {
        name: "Lottery",
        trans: LotteryGroupTrans,
        items: [
            {
                name: Lottery,
                trans: LotteryTrans
            },
            {
                name: LotteryLobby,
                trans: LotteryLobbyTrans
            }
        ]
    }
]

const findTrans = (name, list = AllTrans) => {
    return list.reduce((result, group) => {
        if (result) {
            return result
        }

        if (group.name === name) {
            return group.trans
        }

        if (group.items) {
            return findTrans(name, group.items)
        }
    }, undefined)
}

const findGroup = (name, list = AllTrans) =>
    mapOmit(["items"], list).find((item) => item.name === name)

const findGroupTrans = (name) => findGroup(name)?.trans

export const useGameTypeGroupLabelOf = (gameType = "") => {
    let locale = useLocale()
    let { isLoading, data: groups, error } = useGameTypeGroupList()
    if (isLoading) {
        return ""
    }

    let group = groups?.reduce((result, group) => {
        if (result) {
            return result
        }

        if (group?.items?.includes(gameType)) {
            return group
        }
    }, undefined)

    let trans = findGroupTrans(group?.name)
    return trans?.[locale] ?? group?.name ?? ""
}

export const useGameTypeGroupValueOf = (gameType = "") => {
    let { isLoading, data: groups, error } = useGameTypeGroupList()
    if (isLoading) {
        return ""
    }

    let group = groups?.reduce((result, group) => {
        if (result) {
            return result
        }

        if (group?.items?.includes(gameType)) {
            return group
        }
    }, undefined)

    return group?.name ?? ""
}
/*
export const GameTypeGroupLabel = ({ value }) => {
    let locale = useLocale()
    let trans = findGroupTrans(value)

    return trans?.[locale] ?? value
}
*/

export const GameTypeGroupSelect = ({ placeholder, ...props }) => {
    let defaultPlaceholder = useTrans({
        zh: "游戏类型",
        en: "Game Type"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let locale = useLocale()
    let { isLoading, data: groups } = useGameTypeGroupList()

    let options = groups.map(({ name, ...group }) => {
        let trans = findGroupTrans(name)
        return {
            label: trans?.[locale] ?? name,
            value: name
        }
    })

    return (
        <Select
            placeholder={placeholder}
            isLoading={isLoading}
            options={options}
            {...props}
        />
    )
}

export const GameTypeLabel = ({ value }) => {
    let locale = useLocale()

    return findTrans(value)?.[locale] ?? value
}

export const GameTypeSelect = ({ placeholder, group, ...props }) => {
    let defaultPlaceholder = useTrans({
        zh: "游戏类型",
        en: "Game Type"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let locale = useLocale()
    let { isLoading, data: groups, error } = useGameTypeGroupList()

    let options = groups
        ?.filter(({ name }) => {
            if (group) {
                return group === name
            }
            return true
        })
        .reduce((list, { items, ...group }) => {
            let groupItems = findGroup(group.name)?.items ?? []

            let options = items
                .filter((name) => {
                    return !name.toLowerCase().endsWith("lobby")
                })
                .map((name) => {
                    let trans = findTrans(name, groupItems)
                    return {
                        label: trans?.[locale],
                        value: name
                    }
                })

            return [...list, ...options]
        }, [])

    return (
        <Select
            placeholder={placeholder}
            isLoading={isLoading}
            options={options}
            {...props}
        />
    )
}

// Game Type List include Lobby Type
export const AllGameTypeSelect = ({ placeholder, group, ...props }) => {
    let defaultPlaceholder = useTrans({
        zh: "游戏类型",
        en: "Game Type"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let locale = useLocale()
    let { isLoading, data: groups, error } = useGameTypeGroupList()

    let options = groups
        ?.filter(({ name }) => {
            if (group) {
                return group === name
            }
            return true
        })
        .reduce((list, { items, ...group }) => {
            let options = items.map(({ name, trans }) => {
                return {
                    label: trans?.[locale],
                    value: name
                }
            })

            return [...list, ...options]
        }, [])

    return (
        <Select
            placeholder={placeholder}
            isLoading={isLoading}
            options={options}
            {...props}
        />
    )
}

export default GameType
