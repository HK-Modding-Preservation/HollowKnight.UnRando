﻿# UnRando

A [Randomizer 4](https://github.com/homothetyhk/RandomizerMod) connection which randomizes items by check count only, instead of location.

This connection greatly reduces the rng of randomizer races, allowing "check velocity", or checks-per-minute, to be the only meaningful factor in determining the victor of a race. Two players playing the same seed will always find item X first, item Y second, item Z third, etc., regardless of _where_ they go to find these items.

## Settings

UnRando works on the default split group (-1) by default, but can be changed to work a different split group as well. Only a single split group can be unrando'd.

## Integrations

UnRando is compatible with ItemSync and integrates with RandoSettingsManager.

UnRando is not compatible with MultiWorld (yet). It may not work well with with custom shops, like Lemm shop and Grass shop.

## Details

Shops are hard-coded to only contain a fixed number of checks for balance. Sly, Leg Eater, Salubra and Iselda each sell exactly one item for geo only. Salubra sells 3 more items at specific charm counts, Sly sells 2 more items with shop key, and non-geo shops like Seer, Jiji, and Grubfather "sell" exactly 4 items each.

Sometimes there are more items to obtain than there are locations to find them at, in which case UnRando will stack additional items into the same check(s) to compensate. This means you may sometimes see multiple items for sale in a single slot at a shop.

Lifeblood and Soul refills, once obtained, become permanently located wherever they were obtained. This means that if you see lifeblood at a shop, stag, etc., you can choose not to purchase it, and instead do another check first, knowing that it *will* be lifeblood and the lifeblood will be forever renewable at that location. Use this info wisely.

On the negative side, this also means that if you see a Dreamer at a shop, stag, etc., you cannot judiciously avoid purchasing it to delay infection. Your next check *will* be a Dreamer, no matter what.
